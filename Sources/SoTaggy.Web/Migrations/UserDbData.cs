using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoTaggy.Web.Controllers;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Migrations
{
    public class UserDbData
    {
        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }

        public UserDbData(UserDbContext context)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        public void InitRootData()
        {
            RoleManager.Create(new IdentityRole(AccountController.RoleAnalyst));
            RoleManager.Create(new IdentityRole(AccountController.RoleArtworkAdmin));
            RoleManager.Create(new IdentityRole(AccountController.RoleTagger));
            RoleManager.Create(new IdentityRole(AccountController.RoleTaggingAdmin));
            RoleManager.Create(new IdentityRole(AccountController.RoleUserAdmin));

            CreateUser("root", "p123456", "", "", new string[] { AccountController.RoleUserAdmin });
        }

        private ApplicationUser CreateUser(string userName, string pswd, string firstName, string lastName,
            IEnumerable<string> roles)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
            };
            UserManager.Create(user, pswd);
            user = UserManager.FindByName(userName);

            if (user != null)
            {
                foreach (var role in roles)
                {
                    UserManager.AddToRole(user.Id, role);
                }
            }

            return user;
        }

    }
}