namespace MLGBugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MLGBugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MLGBugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(MLGBugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "MLGsub@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "MLGsub@gmail.com",
                    Email = "MLGsub@gmail.com",
                    FirstName = "MarcAntony",
                    LastName = "La Guerra",
                    DisplayName = "Admin"
                }, "ASPnet123$");
            }

            if (!context.Users.Any(u => u.Email == "projectmanager@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "projectmanager@coderfoundry.com",
                    Email = "projectmanager@coderfoundry.com",
                    FirstName = "Pro",
                    LastName = "Ject",
                    DisplayName = "Project Manager"
                }, "ASPnet123$");
            }

            if (!context.Users.Any(u => u.Email == "developer@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "developer@coderfoundry.com",
                    Email = "developer@coderfoundry.com",
                    FirstName = "Dev",
                    LastName = "Loper",
                    DisplayName = "Developer"
                }, "ASPnet123$");
            }

            if (!context.Users.Any(u => u.Email == "submitter@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "submitter@coderfoundry.com",
                    Email = "submitter@coderfoundry.com",
                    FirstName = "Sub",
                    LastName = "Mitter",
                    DisplayName = "Submitter"
                }, "ASPnet123$");
            }

            var userId = userManager.FindByEmail("MLGsub@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            userId = userManager.FindByEmail("projectmanager@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Project Manager");

            userId = userManager.FindByEmail("developer@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Developer");

            userId = userManager.FindByEmail("submitter@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Submitter");

        }
    }
}
