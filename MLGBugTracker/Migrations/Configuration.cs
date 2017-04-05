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

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
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
                    FirstName = "PM",
                    LastName = "Guy",
                    DisplayName = "ProjectManager"
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
            userManager.AddToRole(userId, "ProjectManager");

            userId = userManager.FindByEmail("developer@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Developer");

            userId = userManager.FindByEmail("submitter@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Submitter");



            if (!context.TicketPriorities.Any(u => u.Name == "High"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "High" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Medium"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Medium" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Low"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Low" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Urgent"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Urgent" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Production Fix"))
            { context.TicketTypes.Add(new TicketType { Name = "Production Fix" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Project Task"))
            { context.TicketTypes.Add(new TicketType { Name = "Project Task" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Software Update"))
            { context.TicketTypes.Add(new TicketType { Name = "Software Update" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "New"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "New" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "In Development"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "In Development" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "Completed"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "Completed" }); }

        }
    }
}