using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugHound.Models;



namespace BugHound.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        private BugHoundSQLEntities db = new BugHoundSQLEntities();

        public ActionResult Index()
        {
            var cu = User.Identity.GetUserName();
            if (cu != "")
            {
                var usrs = db.Users.Single(u => u.UserName == cu);

                //var tickets = db.Tickets.Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type).Include(t => t.User).Include(t => t.User1);
                var tickets = db.Tickets.Where(u => u.AssignedId == usrs.Id && u.StatusId != 2).OrderByDescending(t => t.Priority.SortOrder).Take(5);
                return View(tickets);
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AdministratorPartial()
        {
            //var Message = "Administrator Landing";

            //return PartialView(model: (string)Message);
            //var usrs = db.Users.Single(u => u.UserName == User.Identity.GetUserName());
            //var username = usrs.UserName;

            //var username = "User Name";
            
            return PartialView();
        }

        public ActionResult DeveloperPartial()
        {
            //var usrs = db.Users.Single(u => u.UserName == User.Identity.GetUserName());

            return PartialView();
            
        }

        public ActionResult SupportPartial()
        {
            return PartialView();
        }

        public ActionResult ProjectManagerPartial()
        {
            return PartialView();
        }
    }
}