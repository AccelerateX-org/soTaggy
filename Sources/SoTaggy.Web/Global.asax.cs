using System.Configuration;
using SoTaggy.Web.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SoTaggy.Web.Migrations;
using SoTaggy.Web.Models;

namespace SoTaggy.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // CreateIfDatabaseDoesNotExist
            // eigentlich nur für Release und Produktionsumgebung gedacht
            // => erst einmal ausklammern, da MIgration ja auch Bestandteil des "Publishing" ist
            /*
            var path = Server.MapPath("~/images/Demo");
            Database.SetInitializer<TagDbContext>(new TagDbInitializer(path));
            Database.SetInitializer<UserDbContext>(new UserDbInitializer());
             */

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<UserDbContext, UserDbConfiguration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TagDbContext, TagDbConfiguration>());

        }
    }
}
