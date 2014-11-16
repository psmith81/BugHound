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

        [AllowAnonymous]
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
        
        // Get: Notification
        [AllowAnonymous]
        public ActionResult NotifyPartial()
        {
            var cui = User.Identity.GetUserName();
            var notices = new NotificationViewModel();
            var nc = 0;

            if (cui != "")
            {
                var cu = db.Users.Single(u => u.UserName == cui);
                var dbrst = db.Notifications.Where(u => u.UserId == cu.Id && u.Recieved == false);

                nc = dbrst.Count();
                if (nc == 1)
                {
                    ViewBag.NoticeCount = " " + nc.ToString() + " Notice";
                }
                else
                {
                    ViewBag.NoticeCount = " " + nc.ToString() + " Notices";
                }

                return PartialView();
            }
            else
            {
                ViewBag.NoticeCount = " 0 Notices";
                return PartialView();
            }
        }

        //// POST: Notification
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult NotifyPartial(NotificationViewModel notices)
        //{
        //    return PartialView();
        //}

        // Get: Notifications
        public ActionResult Notifications(int? Id)
        {
            var cui = User.Identity.GetUserName();
            var cu = db.Users.Single(u => u.UserName == cui);
            if (Id != null)
            {
                var acknote = db.Notifications.Single(i => i.Id == Id);
                acknote.Recieved = true;
                db.Entry(acknote).State = EntityState.Modified;
                db.SaveChanges();
            }

            var dbrst = db.Notifications.Where(u => u.UserId == cu.Id && u.Recieved == false);
            var nc = dbrst.Count();
            if (nc == 0)
            {
                return Redirect("/");
            }
            return PartialView(dbrst);
        }

        //[HttpGet]
        //public ActionResult Notificaions(Notification notice)
        //{
        //    return View(notice);
        //}
    }
}