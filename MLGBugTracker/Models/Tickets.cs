using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLGBugTracker.Models
{
    public class Tickets
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusID { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignToUserId { get; set; } 
    }
}