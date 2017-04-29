using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoTaggy.Web.Areas.Admin.Models;
using SoTaggy.Web.Controllers;
using SoTaggy.Web.Data;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    public class TaggingOrgController : Controller
    {
        //
        // GET: /Admin/TaggingOrg/
        public ActionResult Index()
        {
            var db = new TagDbContext();

            var model = db.TaggingOrgs.ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new TaggingOrg();
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult Create(TaggingOrg model)
        {
            var db = new TagDbContext();

            if (ModelState.IsValid)
            {
                db.TaggingOrgs.Add(model);
                db.SaveChanges();

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new UserDbContext()));

                var user = new ApplicationUser() { UserName = model.UserName };
                var result = userManager.Create(user, model.Password);

                user = userManager.FindByName(model.UserName);
                userManager.AddToRole(user.Id, AccountController.RoleTagger);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Start(int id)
        {
            var db = new TagDbContext();

            var org = db.TaggingOrgs.SingleOrDefault(o => o.TaggingOrgId == id);

            var model = new TaggingOrgViewModel {Org = org};

            return View(model);
        }

        public ActionResult Groups(int id)
        {
            var db = new TagDbContext();

            var model = db.TaggingOrgs.SingleOrDefault(org => org.TaggingOrgId == id);
            
            return View(model);
        }

        public JsonResult GroupList(string name)
        {
            var db = new TagDbContext();

            var list = db.TaggingGroups.Where(g => g.Name.Contains(name))
                    .OrderBy(l => l.Name)
                    .Select(l => new
                    {
                        name = l.Name,
                        id = l.TaggingGroupId,
                    })
                    .Take(10);

            return Json(list);
        }

        public PartialViewResult AddGroup(int orgid, string groupname)
        {
            var db = new TagDbContext();

            var org = db.TaggingOrgs.SingleOrDefault(o => o.TaggingOrgId == orgid);
            var group = db.TaggingGroups.SingleOrDefault(g => g.Name.Equals(groupname));

            if (org != null && group != null)
            {
                org.Groups.Add(group);
                db.SaveChanges();
            }

            return PartialView("_GroupList", org);
        }

        [HttpPost]
        public PartialViewResult RemoveGroup(int orgid, int groupid)
        {
            var db = new TagDbContext();

            var org = db.TaggingOrgs.SingleOrDefault(o => o.TaggingOrgId == orgid);
            var group = db.TaggingGroups.SingleOrDefault(g => g.TaggingGroupId == groupid);

            if (org != null && group != null)
            {
                org.Groups.Remove(group);
                db.SaveChanges();
            }

            return PartialView("_EmptyRow");
        }


        public ActionResult EnableTagging(int orgid, int groupid)
        {
            var db = new TagDbContext();

            var org = db.TaggingOrgs.SingleOrDefault(o => o.TaggingOrgId == orgid);
            var group = db.TaggingGroups.SingleOrDefault(g => g.TaggingGroupId == groupid);

            if (org != null && group != null)
            {
                group.CreatedAt = DateTime.Now.AddMinutes(90);
                db.SaveChanges();
            }

            return RedirectToAction("Start", new {id = org.TaggingOrgId});
        }

        public ActionResult DisableTagging(int orgid, int groupid)
        {
            var db = new TagDbContext();

            var org = db.TaggingOrgs.SingleOrDefault(o => o.TaggingOrgId == orgid);
            var group = db.TaggingGroups.SingleOrDefault(g => g.TaggingGroupId == groupid);

            if (org != null && group != null)
            {
                group.CreatedAt = DateTime.Now.AddSeconds(-1);
                db.SaveChanges();
            }

            return RedirectToAction("Start", new {id = org.TaggingOrgId});
        }
    }
}