using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoTaggy.Web.Areas.Admin.Models;
using SoTaggy.Web.Controllers;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "UserAdmin")]
    public class UserAdminController : Controller
    {
        //
        // GET: /UserAdmin/

        private readonly UserDbContext _db;
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<IdentityRole> RoleManager { get; private set; }

        public UserAdminController()
        {
            _db = new UserDbContext();
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));
        }


        public ActionResult Index()
        {
            var userMap = new Dictionary<string, UserModel>();

            var userDb = new UserDbContext();

            var model = new List<UserModel>();

            foreach (var applicationUser in userDb.Users)
            {
                model.Add(new UserModel
                        {
                            UserName = applicationUser.UserName,
                            IsUserAdmin = UserManager.IsInRole(applicationUser.Id, AccountController.RoleUserAdmin),
                            IsAnalyst = UserManager.IsInRole(applicationUser.Id, AccountController.RoleAnalyst),
                            IsArtworkAdmin = UserManager.IsInRole(applicationUser.Id, AccountController.RoleArtworkAdmin),
                            IsTagger = UserManager.IsInRole(applicationUser.Id, AccountController.RoleTagger),
                            IsTaggingAdmin = UserManager.IsInRole(applicationUser.Id, AccountController.RoleTaggingAdmin),
                        }
                    );
            }

            ViewBag.UserCount = userMap.Count;

            return View(model);
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                UserManager.Create(user, model.Password);

                user = UserManager.FindByName(model.UserName);

                CheckRole(user.Id, AccountController.RoleUserAdmin, model.IsUserAdmin);
                CheckRole(user.Id, AccountController.RoleArtworkAdmin, model.IsArtworkAdmin);
                CheckRole(user.Id, AccountController.RoleTaggingAdmin, model.IsTaggingAdmin);
                CheckRole(user.Id, AccountController.RoleAnalyst, model.IsAnalyst);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void CheckRole(string userId, string roleName, bool isInRole)
        {
            if (!RoleManager.RoleExists(roleName))
            {
                RoleManager.Create(new IdentityRole(roleName));   
            }

            if (isInRole)
            {
                UserManager.AddToRole(userId, roleName);
            }
            else
            {
                UserManager.RemoveFromRole(userId, roleName);
            }
        }


        public ActionResult EditUser(string userName)
        {
            var user = UserManager.FindByName(userName);
            string userId = user.Id;

            var model = new UserRegisterModel()
            {
                UserName = userName,
                IsArtworkAdmin = UserManager.IsInRole(userId, AccountController.RoleArtworkAdmin),
                IsTaggingAdmin = UserManager.IsInRole(userId, AccountController.RoleTaggingAdmin),
                IsUserAdmin = UserManager.IsInRole(userId, AccountController.RoleUserAdmin),
                IsAnalyst = UserManager.IsInRole(userId, AccountController.RoleAnalyst),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindByName(model.UserName);
                string userId = user.Id;
                
                CheckRole(userId, AccountController.RoleUserAdmin, model.IsUserAdmin);
                CheckRole(userId, AccountController.RoleArtworkAdmin, model.IsArtworkAdmin);
                CheckRole(userId, AccountController.RoleTaggingAdmin, model.IsTaggingAdmin);
                CheckRole(userId, AccountController.RoleAnalyst, model.IsAnalyst);

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult DeleteUser(string userName)
        {
            if (ModelState.IsValid)
            {
                // alle Benutzer bis auf root löschen
                if (!userName.Equals("root"))
                {
                    var userDb = new UserDbContext();

                    var user = userDb.Users.SingleOrDefault(u => u.UserName.Equals(userName));
                    if (user != null)
                    {
                        userDb.Users.Remove(user);
                        userDb.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChangePassword(string userName)
        {
            var model = new UserRegisterModel()
            {
                UserName = userName,
            };

            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult ResetUAllUser()
        {
            var userDb = new UserDbContext();

            var userList = userDb.Users.ToList();
            foreach (var applicationUser in userList)
            {
                // alle Benutzer bis auf root löschen
                if (!applicationUser.UserName.Equals("root"))
                {
                    userDb.Users.Remove(applicationUser);
                }
            }
            userDb.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
