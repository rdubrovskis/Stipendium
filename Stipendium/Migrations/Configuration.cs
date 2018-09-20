namespace Stipendium.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Stipendium.Models;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

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
            //if (!System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Launch();
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            db.Stiftelses.RemoveRange(db.Stiftelses);

            var Store = new UserStore<ApplicationUser>(context);
            var UserManager = new UserManager<ApplicationUser>(Store);
            var AppUserManager = new ApplicationUserManager(Store);
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            List<string> rolesList = new List<string>
            {
                "Admin",
                "Företag"
            };
            IdentityResult roleResult;

            foreach (var role in rolesList)
            {
                if (!RoleManager.RoleExists(role))
                {
                    roleResult = RoleManager.Create(new IdentityRole(role));

                }

            }


            var admin = new ApplicationUser() { Email = "stiftelseverket@gmail.com", UserName = "stiftelseverket@gmail.com",FirstName="Stiftelseverket",LastName="Administration" };
            var user = db.Users;

            if (AppUserManager.FindByEmail("stiftelseverket@gmail.com") == null)
            {
                UserManager.Create(admin, "admin1");
                //context.Users.AddOrUpdate(admin);
                
            }
            if(UserManager.FindByEmail("stiftelseverket@gmail.com").Roles.Count == 0)
            {
                UserManager.AddToRole(UserManager.FindByEmail("stiftelseverket@gmail.com").Id, "Admin");
            }


        }
    }
}
