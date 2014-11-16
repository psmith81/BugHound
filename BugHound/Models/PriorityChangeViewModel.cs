using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugHound.Models
{
    public class PriorityChangeViewModel
    {
        [Required]
        public int TicketId { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public int PriorityId { get; set; }
    }
}