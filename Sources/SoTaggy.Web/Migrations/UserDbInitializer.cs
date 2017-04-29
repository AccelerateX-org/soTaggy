using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SoTaggy.Web.Models;

namespace SoTaggy.Web.Migrations
{
    public class UserDbInitializer : CreateDatabaseIfNotExists<UserDbContext>
    {
        protected override void Seed(UserDbContext context)
        {
            new UserDbData(context).InitRootData();
        }
    }
}