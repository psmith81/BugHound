//------------------------------------------------
// pbs: Constructors to set property values.
//
// BugHound
// Written: Nov.-14, 2014
// Coder Foundry Master Class MVC Project
//------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugHound.Models
{
    public partial class Notification
    {
        public Notification()
        {
            this.SentAt = DateTime.UtcNow;
            this.Recieved = false;
        }

        public Notification(int TicketId, int UserId, string Notification)
            : this()
        {
            this.TicketId = TicketId;
            this.UserId = UserId;
            this.Notification1 = Notification;
        }
    }
}