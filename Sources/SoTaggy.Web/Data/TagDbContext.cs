using System.Data.Entity;

namespace SoTaggy.Web.Data
{
    public class TagDbContext : DbContext
    {
        public TagDbContext() : base("TagDb")
        {
            
        }

        #region Kataloge (Stammdaten)
        // Sammlungen
        public DbSet<ArtworkLibrary> ArtworkLibraries { get; set; }

        // Bildrechteinhaber
        public DbSet<CopyrightOwner> CopyrightOwners { get; set; }
        #endregion

        #region Bilder und Tags
        public DbSet<ArtworkPool> ArtworkPools { get; set; }

        public DbSet<Artwork> Artworks { get; set; }

        public DbSet<ArtworkImage> Images { get; set; }

        public DbSet<ArtworkTag> Taggings { get; set; }

        #endregion

        #region Taggingprojekte (temporäre Daten)

        public DbSet<TaggingOrg> TaggingOrgs { get; set; }

        public DbSet<TaggingGroup> TaggingGroups { get; set; }

        public DbSet<GroupTag> GroupTags { get; set; }

        #endregion

    }
}
