using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Migrations
{
    public class TagDbInitializer : CreateDatabaseIfNotExists<TagDbContext>
    {
        private string _homeDir;

        public TagDbInitializer(string path)
        {
            _homeDir = path;
        }

        protected override void Seed(TagDbContext context)
        {
            var data = new TagDbData(context);
            data.InitCatalogData();
            data.InitDemoPool(_homeDir);
        }
    }
}