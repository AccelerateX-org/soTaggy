namespace SoTaggy.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class TagDbConfiguration : DbMigrationsConfiguration<SoTaggy.Web.Data.TagDbContext>
    {
        public TagDbConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(SoTaggy.Web.Data.TagDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            //new TagDbData(context).InitCatalogData();

            // Problem: hier kenne ich das Verzeichnis für den Demopool nicht
            // Daher kann man die Daten hier nicht einlesen.
            // Muss als Admin-Funktion gebaut werden.
        }
    }
}
