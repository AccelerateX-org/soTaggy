using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Migrations
{
    public class TagDbData
    {
        private TagDbContext _context;

        public TagDbData(TagDbContext context)
        {
            _context = context;
        }

        public void InitCatalogData()
        {
            _context.ArtworkLibraries.AddOrUpdate(l => l.ShortName,
                new ArtworkLibrary() {Name = "Alte Pinakothek", ShortName = "AP"},
                new ArtworkLibrary() {Name = "Neue Pinakothek", ShortName = "NP"},
                new ArtworkLibrary() {Name = "Pinakothek der Moderne", ShortName = "PdM"}
                );

            _context.CopyrightOwners.AddOrUpdate(c => c.ShortName,
                new CopyrightOwner() { Name="Bayerische Staatsgemäldesammlungen", ShortName="BYSGM" });
        }

        public void InitDemoPool(string homeDir)
        {
            var pool =_context.ArtworkPools.SingleOrDefault(p => p.Name.Equals("Demo"));
            if (pool != null)
                return;

            InitCatalogData();

            pool = new ArtworkPool()
            {
                Name = "Demo",
                Description = "Demodateien",
            };

            _context.ArtworkPools.Add(pool);
            _context.SaveChanges();

            var lines = File.ReadAllLines(Path.Combine(homeDir, "Index.csv"), Encoding.Default);
            foreach (var line in lines)
            {
                var elem = line.Split(';');

                var libName = elem[6];
                var cpName = elem[7];

                var library = _context.ArtworkLibraries.SingleOrDefault(l => l.ShortName.Equals(libName));
                var copy = _context.CopyrightOwners.SingleOrDefault(c => c.ShortName.Equals(cpName));

                var artwork = new Artwork()
                {
                    ArtworkPoolId = pool.ArtworkPoolId,
                    Artist = elem[1],
                    Title = elem[2],
                    Dating = elem[3],
                    Technique = elem[4],
                    Dimensions = elem[5],
                    ArtworkLibrary = library,
                    CopyrightOwner = copy,
                };

                _context.Artworks.Add(artwork);

                var imageFile = Path.Combine(homeDir, elem[0]);

                var image = new ArtworkImage
                {
                    ImageFileType = "image/" + Path.GetExtension(imageFile).Remove(0, 1),
                    Artwork = artwork,
                    ImageData = File.ReadAllBytes(imageFile),
                };

                _context.Images.Add(image);
                _context.SaveChanges();
            }

            _context.SaveChanges();
            
        }
    }
}