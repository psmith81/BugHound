using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugHound.Models;
using Microsoft.AspNet.Identity;


namespace BugHound.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private BugHoundSQLEntities db = new BugHoundSQLEntities();

        // GET: Tickets
        public ActionResult Index()
        {
            //var tickets = db.Tickets.Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type).Include(t => t.User).Include(t => t.User1);
            var cu = User.Identity.GetUserName();
            var usrs = db.Users.Single(u => u.UserName == cu);
            
            var tickets = db.Tickets.Where(u => u.AssignedId == usrs.Id && u.StatusId != 2).OrderByDescending(p => p.PriorityId);

            if (User.IsInRole("Administrator"))
            {
                 tickets = db.Tickets.OrderByDescending(p => p.PriorityId);
                return View(tickets.ToList());
            }
            if (User.IsInRole("Project Manager"))
            {
                tickets = db.Tickets.Where(u => u.AssignedId == usrs.Id || u.ProjectId == usrs.Id).OrderByDescending(p => p.PriorityId);
                return View(tickets.ToList());
            }

            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.PreviousPage = Request.UrlReferrer.AbsolutePath.ToString();
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Administrator, Project Manager, Developer, Support")]
        public ActionResult Create()
        {
            ViewBag.PriorityId = new SelectList(db.Priorities.OrderBy(so => so.SortOrder), "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.StatusId = new SelectList(db.Status.OrderBy(so => so.SortOrder), "Id", "Name");
            ViewBag.TypeId = new SelectList(db.Types.OrderBy(so => so.SortOrder), "Id", "Name");
            ViewBag.AssignedId = new SelectList(db.Users.Where(ul => ul.Active).OrderBy(so => so.Name), "Id", "Name");
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager, Developer, Support")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EnteredById,CreatedOn,PriorityId,Title,TypeId,StatusId,ProjectId,AssignedId,Narration,Attachments")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var cu = User.Identity.GetUserName();
                var usrs = db.Users.Single(u => u.UserName  == cu);

                ticket.EnteredById = usrs.Id;
                ticket.CreatedOn =  DateTime.Now;
                ticket.LastedUpdated = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "Name", ticket.AssignedId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "Name", ticket.EnteredById);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "Name", ticket.AssignedId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "Name", ticket.EnteredById);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EnteredById,CreatedOn,PriorityId,Title,TypeId,StatusId,ProjectId,AssignedId,LastedUpdated,Narration,Attachments")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PriorityId = new SelectList(db.Priorities, "Id", "Name", ticket.PriorityId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "Name", ticket.StatusId);
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", ticket.TypeId);
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "Name", ticket.AssignedId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "Name", ticket.EnteredById);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
