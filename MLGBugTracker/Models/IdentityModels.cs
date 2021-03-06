﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using MLGBugTracker.Models;

namespace MLGBugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Project = new HashSet<Project>();
        }

        public string DisplayName { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string FullName { get; internal set; }


        public virtual ICollection<Project>Project { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Name", DisplayName));
            //userIdentity.AddClaim(new Claim("Name", FullName));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<MLGBugTracker.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<MLGBugTracker.Models.Ticket> Tickets { get; set; }

        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketPriority> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketStatus> TicketStatuses { get; set; }
        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketNotification> TicketNotifications { get; set; }
        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketType> TicketTypes { get; set; }
        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketHistory> TicketHistories { get; set; }
        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketComment> TicketComments { get; set; }
        public System.Data.Entity.DbSet<MLGBugTracker.Models.TicketAttachment> TicketAttachments { get; set; }

    }
}