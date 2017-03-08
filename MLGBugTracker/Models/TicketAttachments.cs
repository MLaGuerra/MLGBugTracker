using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLGBugTracker.Models
{
    public class TicketAttachments
    {
        public int Id { get; set; }
        public string TickeId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }
    }
}