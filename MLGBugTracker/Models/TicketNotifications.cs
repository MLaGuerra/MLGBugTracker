using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLGBugTracker.Models
{
    public class TicketNotifications
    {
        public int Id { get; set; }
        public string TicketId { get; set; }
        public string UserId { get; set; }

    }
}