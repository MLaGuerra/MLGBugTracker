using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLGBugTracker.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public IEnumerable<SelectListItem> AllProjectUsers { get; set; }

        private List<string> _selectedProjectUsers;
        public List<string> SelectedProjectUsers
        {
            get
            {
                if (_selectedProjectUsers == null)
                {
                    _selectedProjectUsers = Project.Users.Select(m => m.Id).ToList();
                }
                return _selectedProjectUsers;
            }
            set { _selectedProjectUsers = value; }
        }
    }
}