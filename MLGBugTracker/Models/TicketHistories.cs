﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MLGBugTracker.Models
{
    public class TicketHistories
    {
        public int Id { get; set; }
        public string TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime Changed { get; set; }
        public string UserId { get; set; }
    }
}