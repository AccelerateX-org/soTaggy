using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class LibraryController : Controller
    {
        private readonly TagDbContext db = new TagDbContext();

        //
        // GET: /Library/

        public ActionResult Index()
        {
            return View(db.ArtworkLibraries.ToList());
        }

        //
        // GET: /Library/Details/5

        public ActionResult Details(int id = 0)
        {
            var artworklibrary = db.ArtworkLibraries.Find(id);
            if (artworklibrary == null)
            {
                return HttpNotFound();
            }
            return View(artworklibrary);
        }

        //
        // GET: /Library/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Library/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArtworkLibrary artworklibrary)
        {
            if (ModelState.IsValid)
            {
                db.ArtworkLibraries.Add(artworklibrary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artworklibrary);
        }

        //
        // GET: /Library/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var artworklibrary = db.ArtworkLibraries.Find(id);
            if (artworklibrary == null)
            {
                return HttpNotFound();
            }
            return View(artworklibrary);
        }

        //
        // POST: /Library/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArtworkLibrary artworklibrary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artworklibrary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artworklibrary);
        }

        //
        // GET: /Library/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var artworklibrary = db.ArtworkLibraries.Find(id);
            if (artworklibrary == null)
            {
                return HttpNotFound();
            }
            return View(artworklibrary);
        }

        //
        // POST: /Library/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var artworklibrary = db.ArtworkLibraries.Find(id);
            db.ArtworkLibraries.Remove(artworklibrary);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}