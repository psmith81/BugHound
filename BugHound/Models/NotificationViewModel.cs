using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugHound.Models
{
    public partial class NotificationViewModel
    {
        public int[] Id { get; set; }
        public int[] UserId { get; set; }
        public int[] TicketId { get; set; }
        public string[] Notification1 { get; set; }
        public int[] MyProperty { get; set; }


        //public int count { get; set; }
    }
}