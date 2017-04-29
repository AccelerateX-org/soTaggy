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
    public class CopyrightController : Controller
    {
        private TagDbContext db = new TagDbContext();

        //
        // GET: /Copyright/

        public ActionResult Index()
        {
            return View(db.CopyrightOwners.ToList());
        }

        //
        // GET: /Copyright/Details/5

        public ActionResult Details(int id = 0)
        {
            var copyrightowner = db.CopyrightOwners.Find(id);
            if (copyrightowner == null)
            {
                return HttpNotFound();
            }
            return View(copyrightowner);
        }

        //
        // GET: /Copyright/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Copyright/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CopyrightOwner copyrightowner)
        {
            if (ModelState.IsValid)
            {
                db.CopyrightOwners.Add(copyrightowner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(copyrightowner);
        }

        //
        // GET: /Copyright/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var copyrightowner = db.CopyrightOwners.Find(id);
            if (copyrightowner == null)
            {
                return HttpNotFound();
            }
            return View(copyrightowner);
        }

        //
        // POST: /Copyright/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CopyrightOwner copyrightowner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(copyrightowner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(copyrightowner);
        }

        //
        // GET: /Copyright/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var copyrightowner = db.CopyrightOwners.Find(id);
            if (copyrightowner == null)
            {
                return HttpNotFound();
            }
            return View(copyrightowner);
        }

        //
        // POST: /Copyright/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var copyrightowner = db.CopyrightOwners.Find(id);
            db.CopyrightOwners.Remove(copyrightowner);
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