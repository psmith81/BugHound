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
    //[Authorize(Roles = "Administrator")]
    [Authorize]    
    public class CommentsController : Controller
    {
        private BugHoundSQLEntities db = new BugHoundSQLEntities();
        public int ticket;

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Ticket).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int? ticketid)
        {
            //ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
            //var ticket = tick;
            if (ticketid == null)
            {
                ViewBag.tickettitle = "---";
            }
            else
            {
                var ct = db.Tickets.Single(i => i.Id == ticketid);
                ViewBag.tickettitle = ct.Title;
            }
            ViewBag.PreviousPage = Request.UrlReferrer.AbsolutePath.ToString();
            
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,TicketId,Comment1")] Comment comment)
        {
            if (ModelState.IsValid )
            {
                var cu = User.Identity.GetUserName();
                var userId = db.Users.Single(u => u.UserName == cu).Id;

                comment.UserId = userId;

                db.Comments.Add(comment);
                db.SaveChanges();
                
                //Update Last Updated Field
                var ct = db.Tickets.Single(id => id.Id == comment.TicketId);
                ct.LastedUpdated = DateTime.Now;
                db.Entry(ct).State = EntityState.Modified;
                db.SaveChanges();


                // Create History Event
                var he = new History(ct.Id, userId, "A comment regarding Ticket Id:" + ct.Id.ToString() + " has been submitted.");
                
                db.Histories.Add(he);
                db.SaveChanges();
				

                return RedirectToAction("../Tickets/Details/" + comment.TicketId);
            }

            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            //ViewBag.UserId = new SelectList(db.Users, "Id", "Name", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,TicketId,Comment1")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", comment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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

        // GET: StatusChange
        [Authorize(Roles = "Administrator, Project Manager")]
        public ActionResult StatusChange(int? ticketid)
        {
            var ct = new StatusChangeViewModel();
            if (ticketid == null)
            {
                ViewBag.tickettitle = "---";
            }
            else
            {
                var tick = db.Tickets.Single(i => i.Id == ticketid);
                ViewBag.tickettitle = tick.Title;
                ct.TicketId = ticketid.GetValueOrDefault();
            }

            ViewBag.StatusId = new SelectList(db.Status.OrderBy(so => so.SortOrder), "Id", "Name");
            ViewBag.PreviousPage = Request.UrlReferrer.AbsolutePath.ToString();
            ViewBag.SCMessage = "";

            return View(ct);
        }

        // POST: Ticketes/StatusChange
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult StatusChange(StatusChangeViewModel statchange)
        {
            var comment = new Comment();
                
            if (ModelState.IsValid)
            {
                var cu = User.Identity.GetUserName();
                var usrs = this.db.Users.Single(u => u.UserName == cu).Id;

                comment.UserId = usrs;
                comment.TicketId = statchange.TicketId;
                comment.Comment1 = statchange.Comment;


                db.Comments.Add(comment);
                db.SaveChanges();

                var ct = db.Tickets.Single(id => id.Id == comment.TicketId);
                var oldstat = ct.Status.Name;
                ct.StatusId = statchange.StatusId;
                ct.LastedUpdated = DateTime.Now;
                db.Entry(ct).State = EntityState.Modified;
                db.SaveChanges();

                // Create History Event
                var he = new History(ct.Id, usrs, "Status was changed from \"" + oldstat + "\" to \"" + ct.Status.Name + "\".");

                db.Histories.Add(he);
                db.SaveChanges();

                return RedirectToAction("../Tickets/Details/" + comment.TicketId);
            }

            var tick = db.Tickets.Single(i => i.Id == statchange.TicketId);
            ViewBag.tickettitle = tick.Title;
                
            ViewBag.StatusId = new SelectList(db.Status.OrderBy(so => so.SortOrder), "Id", "Name");
            return View(statchange);
        }

    }
}
