using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLGBugTracker.Models
{
    public class AdminProjectViewModel
    {
        public Projects Project { get; set; }
        public SelectList PMUsers { get; set; }
        public string SelectedUser { get; set; }
    }
}