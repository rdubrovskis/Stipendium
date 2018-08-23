namespace Stipendium.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Stipendium.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Stipendium.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void Seed(Stipendium.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            for (int i = 0; i < 10; i++)
            {
                var seedStipend = new Stipend
                {
                    Title = "Test Stipend " + i.ToString(),
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam consectetur elit sed sagittis convallis. Pellentesque ut mattis nisl. " +
                    "Duis ornare tortor eu dui dapibus luctus. Morbi faucibus ex velit, placerat ullamcorper diam rhoncus vitae. Sed magna turpis, elementum non tincidunt in, " +
                    "lobortis rhoncus arcu. Nunc et venenatis elit. Praesent non felis nisi. Suspendisse egestas nisi nunc, at lobortis turpis ornare id. Sed condimentum ante et " +
                    "tempus vestibulum. Aenean nec sapien ornare, tempor sapien non, pulvinar velit.",
                    OrgNr = "1234567-" + i.ToString(),
                    AddressLine1 = "Address Line 1 for Test-" + i.ToString(),
                    Capital = i * 1m,
                    ID = "guid-would-usually-go-here-" + i.ToString(),
                    ContactInfo = "0712345" + i.ToString(),
                    PostNr = 10001 + i,
                    AcceptsApplications = true,
                    County = "Stockholms Län"
                };

                db.Stipends.AddOrUpdate(seedStipend);
                db.SaveChanges();

            }
        }
    }
}
