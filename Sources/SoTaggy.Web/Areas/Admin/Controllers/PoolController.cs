using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoTaggy.Web.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using SoTaggy.Web.Migrations;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class PoolController : Controller
    {
        private TagDbContext db = new TagDbContext();

        //
        // GET: /ImagePool/

        public ActionResult Index()
        {
            ViewBag.DemoAvailable = db.ArtworkPools.SingleOrDefault(p => p.Name.Equals("Demo")) != null;
            
            return View(db.ArtworkPools.Where(m => string.IsNullOrEmpty(m.Name) == false).ToList());
        }


        public ActionResult InsertDemo()
        {
            var path = Server.MapPath("~/images/Demo");

            new TagDbData(new TagDbContext()).InitDemoPool(path);
            
            return RedirectToAction("Index");
        }

        //
        // GET: /ImagePool/Details/5

        public ActionResult Details(int id = 0)
        {
            var imagepool = db.ArtworkPools.Find(id);
            if (imagepool == null)
            {
                return HttpNotFound();
            }
            return View(imagepool);
        }

        //
        // GET: /ImagePool/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ImagePool/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArtworkPool imagepool)
        {
            if (ModelState.IsValid)
            {
                db.ArtworkPools.Add(imagepool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(imagepool);
        }

        //
        // GET: /ImagePool/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var imagepool = db.ArtworkPools.Find(id);
            if (imagepool == null)
            {
                return HttpNotFound();
            }
            return View(imagepool);
        }

        //
        // POST: /ImagePool/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArtworkPool imagepool)
        {
            if (ModelState.IsValid)
            {
                db.Entry(imagepool).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(imagepool);
        }

        //
        // GET: /ImagePool/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var imagepool = db.ArtworkPools.Find(id);
            if (imagepool == null)
            {
                return HttpNotFound();
            }
            return View(imagepool);
        }

        //
        // POST: /ImagePool/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var imagepool = db.ArtworkPools.Find(id);
            
            db.ArtworkPools.Remove(imagepool);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ImageList(int id = 0)
        {
            var imagepool = db.ArtworkPools.Find(id);
            if (imagepool == null)
            {
                return HttpNotFound();
            }

            ViewBag.ImagePoolID = imagepool.ArtworkPoolId;
            ViewBag.ImagePoolName = imagepool.Name;

            return View(imagepool.Artworks);
        }

        public ActionResult CreateImage(int id = 0)
        {
            var imagepool = db.ArtworkPools.Find(id);
            if (imagepool == null)
            {
                return HttpNotFound();
            }

            var imageentry = new Artwork() { 
                ArtworkPoolId = imagepool.ArtworkPoolId,
            };

            // Die Auswahllisten
            ViewBag.Libraries = new SelectList(db.ArtworkLibraries.ToList(), "ArtworkLibraryID", "Name");
            ViewBag.Copyrights = new SelectList(db.CopyrightOwners.ToList(), "CopyrightOwnerID", "Name");


            return View(imageentry);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateImage(Artwork imageentry)
        {
            if (ModelState.IsValid)
            {
                db.Artworks.Add(imageentry);
                db.SaveChanges();
                return RedirectToAction("ImageList", new { id = imageentry.ArtworkPoolId });
            }

            return View(imageentry);
        }

        public ActionResult UploadImage(int id = 0)
        {
            var image = db.Artworks.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }

            return View(image);
        }

        
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase uploadFile, int ArtworkID)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null)
                {
                    var artwork = db.Artworks.Find(ArtworkID);

                    if (artwork != null)
                    {
                        var image = new ArtworkImage
                        {
                            ImageFileType = uploadFile.ContentType,
                            ImageData = new byte[uploadFile.ContentLength],
                        };

                        // TODO: das Bild auch speichern
                        uploadFile.InputStream.Read(image.ImageData, 0, uploadFile.ContentLength);

                        db.Images.Add(image);
                        artwork.Images.Add(image);

                        db.SaveChanges();


                        return RedirectToAction("ImageList", new { id = artwork.ArtworkPoolId });
                    }
                }
            }
            
            return View();
        }

        public ActionResult EditImage(int id = 0)
        {
            var image = db.Artworks.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }

            // Die Auswahllisten
            ViewBag.Libraries = new SelectList(db.ArtworkLibraries.ToList(), "ArtworkLibraryID", "Name", image.ArtworkLibrary);
            ViewBag.Copyrights = new SelectList(db.CopyrightOwners.ToList(), "CopyrightOwnerID", "Name", image.CopyrightOwner);

            return View(image);
        }

        //
        // POST: /ImagePool/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImage(Artwork image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ImageList", new { id = image.ArtworkPoolId });
            }
            return View(image);
        }

        public ActionResult DeleteImage(int id = 0)
        {
            var image = db.Artworks.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        //
        // POST: /ImagePool/Delete/5

        [HttpPost, ActionName("DeleteImage")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteImageConfirmed(int id)
        {
            var artwork = db.Artworks.Find(id);

            foreach (var image in artwork.Images.ToList())
            {
                db.Images.Remove(image);
            }
            
            db.Artworks.Remove(artwork);
            db.SaveChanges();


            return RedirectToAction("ImageList", new { id = artwork.ArtworkPoolId });
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}