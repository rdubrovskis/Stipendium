namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Stipendium.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<Stipendium.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Stipendium.Models.ApplicationDbContext";
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void Seed(Stipendium.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //for (int i = 0; i < 10; i++)
            //{
            //    Stipend seededStipend = new Stipend
            //    {
            //        Title = "Stipend" + i.ToString(),
            //        AddressLine1 = "Address Line1 for Stipend" + i.ToString(),
            //        Description = "This is the description for Stipend" + i.ToString(),
            //        OrgNr = "000000-" + i.ToString(),
            //        PostNr= 1001 + i,
            //        Capital = i * 1m
            //    };
            //    db.Stipends.AddOrUpdate(seededStipend);
            //}
            //db.SaveChanges();
        }
    }
}
