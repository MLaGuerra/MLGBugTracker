using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLGBugTracker.Models
{
    public class ProjectPMViewModel
    {
        public Projects Project { get; set; }
        public ApplicationUser ProjectManager { get; set; }
    }
}