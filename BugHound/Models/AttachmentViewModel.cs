using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugHound.Models
{
    public class AttachmentViewModel
    {
        public int TicketId { get; set; }
        public string Description { get; set; }
        public HttpPostedFileBase FileObj { get; set; }
    }
}