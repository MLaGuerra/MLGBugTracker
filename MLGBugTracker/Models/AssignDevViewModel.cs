using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLGBugTracker.Models
{
    public class AssignDevViewModel
    {
        public Ticket Ticket { get; set; }
        public SelectList Developers { get; set; }
        public string SelectedUser { get; set; }
    }
}