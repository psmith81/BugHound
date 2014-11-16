using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugHound.Models
{
    public partial class History
    {
        public History()
        {
            this.Occured = DateTime.Now;
        }

        public History(int TicketId, int UserId, string Event) : this()
        {
            this.TicketId = TicketId;
            this.UserId = UserId;
            this.Event = Event;
        }
    }
}