using MLGBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace MLGBugTracker.Helpers
{
    public static class HelperExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static string GetName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "Name");
            if (claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }
    }
}