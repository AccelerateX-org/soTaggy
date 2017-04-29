using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoTaggy.Web.Controllers;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class UserDbConfiguration : DbMigrationsConfiguration<SoTaggy.Web.Models.UserDbContext>
    {


        public UserDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(SoTaggy.Web.Models.UserDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //new UserDbData(context).InitRootData();
        }
    }
}
