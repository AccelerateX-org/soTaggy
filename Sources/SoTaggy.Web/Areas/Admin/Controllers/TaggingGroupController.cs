using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoTaggy.Web.Data;
using SoTaggy.Web.Areas.Admin.Models;
using SoTaggy.Web.Controllers;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class TaggingGroupController : Controller
    {
        private readonly TagDbContext db = new TagDbContext();

        //
        // GET: /Admin/TaggingGroup/

        public ActionResult Index()
        {
            var tagginggroups = db.TaggingGroups.Include(t => t.ArtworkPool);
            return View(tagginggroups.ToList());
        }

        //
        // GET: /Admin/TaggingGroup/Details/5

        public ActionResult Details(int id = 0)
        {
            var tagginggroup = db.TaggingGroups.Find(id);
            if (tagginggroup == null)
            {
                return HttpNotFound();
            }
            return View(tagginggroup);
        }

        //
        // GET: /Admin/TaggingGroup/Create

        public ActionResult Create()
        {
            ViewBag.ArtworkPoolId = new SelectList(db.ArtworkPools, "ArtworkPoolId", "Name");
            return View();
        }

        //
        // POST: /Admin/TaggingGroup/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaggingGroup tagginggroup)
        {
            if (ModelState.IsValid)
            {
                tagginggroup.Owner = User.Identity.Name;
                tagginggroup.CreatedAt = DateTime.Now;

                db.TaggingGroups.Add(tagginggroup);
                db.SaveChanges();

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new UserDbContext()));

                var user = new ApplicationUser() { UserName = tagginggroup.UserName };
                var result = userManager.Create(user, tagginggroup.Password);

                user = userManager.FindByName(tagginggroup.UserName);
                userManager.AddToRole(user.Id, AccountController.RoleTagger);

                return RedirectToAction("Index");
            }

            ViewBag.ArtworkPoolId = new SelectList(db.ArtworkPools, "ArtworkPoolId", "Name", tagginggroup.ArtworkPoolId);
            return View(tagginggroup);
        }

        //
        // GET: /Admin/TaggingGroup/Edit/5

        public ActionResult Edit(int id = 0)
        {
            var tagginggroup = db.TaggingGroups.Find(id);
            if (tagginggroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.ArtworkPoolId = new SelectList(db.ArtworkPools, "ArtworkPoolId", "Name", tagginggroup.ArtworkPoolId);
            return View(tagginggroup);
        }

        //
        // POST: /Admin/TaggingGroup/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaggingGroup model)
        {
            if (ModelState.IsValid)
            {
                var tagginggroup = db.TaggingGroups.Find(model.TaggingGroupId);

                tagginggroup.Name = model.Name;
                tagginggroup.UnitStart = model.UnitStart;
                tagginggroup.UnitEnd = model.UnitEnd;

                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ArtworkPoolId = new SelectList(db.ArtworkPools, "ArtworkPoolId", "Name", model.ArtworkPoolId);
            return View(model);
        }

        //
        // GET: /Admin/TaggingGroup/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var tagginggroup = db.TaggingGroups.Find(id);
            if (tagginggroup == null)
            {
                return HttpNotFound();
            }

            return View(tagginggroup);
        }

        //
        // POST: /Admin/TaggingGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tagginggroup = db.TaggingGroups.Find(id);
            db.TaggingGroups.Remove(tagginggroup);
            db.SaveChanges();

            var userDb = new UserDbContext();

            var user = userDb.Users.SingleOrDefault(u => u.UserName.Equals(tagginggroup.UserName));
            if (user != null)
            {
                userDb.Users.Remove(user);
                userDb.SaveChanges();
            }

            
            return RedirectToAction("Index");
        }

        public ActionResult Tags(int id)
        {
            var tagginggroup = db.TaggingGroups.Find(id);

            var model = new GroupTagModel
            {
                Group = tagginggroup,
                GroupId = tagginggroup.TaggingGroupId
            };

            return View(model);

        }


        [HttpPost]
        public PartialViewResult SendTag(GroupTagModel model)
        {
            var tag = db.GroupTags.SingleOrDefault(t => t.Tag.Equals(model.Tag));
            var tagginggroup = db.TaggingGroups.Find(model.GroupId);

            if (tag == null)
            {
                tag = new GroupTag()
                {
                    Tag = model.Tag,
                };
                db.GroupTags.Add(tag);
            }

            if (!tagginggroup.GroupTags.Contains(tag))
            {
                tagginggroup.GroupTags.Add(tag);
                //tag.Groups.Add(tagginggroup);
            }
            
            db.SaveChanges();

            // Das Modell für die Anzeige der Liste aktualisieren
            return PartialView("_GroupTagList", tagginggroup);
        }

        [HttpPost]
        public PartialViewResult DeleteTag(int tagId, int groupId)
        {
            // zuerst den Tag speichern
            var tag = db.GroupTags.SingleOrDefault(t => t.GroupTagId == tagId);
            var group = db.TaggingGroups.SingleOrDefault(g => g.TaggingGroupId == groupId);

            if (tag != null && group != null)
            {
                group.GroupTags.Remove(tag);
                db.SaveChanges();

            }

            // Das Modell für die Anzeige der Liste aktualisieren
            return PartialView("_GroupTagList", group);
        }




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}