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
using System.IO;
using PagedList;


namespace BugHound.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private BugHoundSQLEntities db = new BugHoundSQLEntities();

        // GET: Tickets
        public ActionResult Index(int? page)
        {
            //var tickets = db.Tickets.Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Include(t => t.Type).Include(t => t.User).Include(t => t.User1);
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var cu = User.Identity.GetUserName();
            var usrs = db.Users.Single(u => u.UserName == cu);
            
            var tickets = db.Tickets.Where(u => u.AssignedId == usrs.Id && u.StatusId != 2).OrderByDescending(p => p.PriorityId);

            if (User.IsInRole("Administrator"))  // Returns all tickets
            {
                 tickets = db.Tickets.OrderByDescending(p => p.PriorityId);
                return View(tickets.ToPagedList(pageNumber, pageSize));
            }
            if (User.IsInRole("Project Manager")) // Returns Tickets owned user and all tickets where user is the PM.
            {
                tickets = db.Tickets.Where(u => u.AssignedId == usrs.Id || u.Project.ManagerId == usrs.Id).OrderByDescending(p => p.PriorityId);
                return View(tickets.ToPagedList(pageNumber, pageSize));
            }

            return View(tickets.ToPagedList(pageNumber, pageSize));  // Returns tickets assigned to user.
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            ticket.Comments = ticket.Comments.OrderByDescending(m => m.Id).ToList();
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
        public ActionResult Create([Bind(Include = "EnteredById,CreatedOn,PriorityId,Title,TypeId,StatusId,ProjectId,AssignedId,Narration,Attachments")] Ticket ticket)
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

                // Create History Event
                var eventstr =  "Ticket Id:" + ticket.Id.ToString() + " was submitted.";
                var he = new History(ticket.Id, usrs.Id, eventstr);
                db.Histories.Add(he);
                db.SaveChanges();

                var ne = new Notification(ticket.Id, ticket.AssignedId, eventstr);
                db.Notifications.Add(ne);
                db.SaveChanges();

                var nep = new Notification(ticket.Id, ticket.User1.Id, eventstr);
                db.Notifications.Add(nep);
                db.SaveChanges();

                return RedirectToAction("Details/" + ticket.Id);
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

        // GET: Attachements/Create
        [Authorize(Roles = "Administrator, Project Manager, Developer, Support")]
        public ActionResult AttachCreate(int? ticketid)
        {
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

        // POST: Attachements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator, Project Manager, Developer, Support")]
        [ValidateAntiForgeryToken]
        public ActionResult AttachCreate(AttachmentViewModel attachmentVm)
        {
            if (ModelState.IsValid)
            {

                if (attachmentVm.FileObj != null && attachmentVm.FileObj.ContentLength > 0)
                {
                    var cu = User.Identity.GetUserName();
                    var usr = db.Users.Single(u => u.UserName == cu).Id;
                    var attachment = new Attachement()
                    {
                        UserId = usr,
                        DateAttached = DateTime.Now,
                        FileUNQName = Guid.NewGuid().ToString() + attachmentVm.FileObj.FileName,
                        Description = attachmentVm.Description,
                        FileName = attachmentVm.FileObj.FileName,
                        TicketId = attachmentVm.TicketId,
                        UpLoaded = true
                    };

                    var path = HttpContext.Server.MapPath("~/Repository/Attachments/" + attachmentVm.TicketId + "/");
                    var fullName = Path.Combine(path, attachment.FileUNQName);

                    if (!Directory.Exists(path))
                    {
                        //Server.MapPath(path);
                        Directory.CreateDirectory(path);
                    }

                    attachmentVm.FileObj.SaveAs(fullName);

                    db.Attachements.Add(attachment);
                    db.SaveChanges();

                    //Update Last Updated Field
                    var ct = db.Tickets.Single(id => id.Id == attachmentVm.TicketId);
                    ct.LastedUpdated = DateTime.Now;
                    db.Entry(ct).State = EntityState.Modified;
                    db.SaveChanges();

                    // Create History Event
                    var he = new History(ct.Id, usr, 
                        "Attachment, ID No." + attachment.Id.ToString() + ", made associated with Ticket No." + attachmentVm.TicketId +"");
                    
                    db.Histories.Add(he);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachment.TicketId);
                //ViewBag.UserId = new SelectList(db.Users, "Id", "Name", attachment.UserId);
                //return View(attachment);
            }
            return View();

        }

        public ActionResult Download(int FileId)
        {
            //var document = db.Attachements.Single(a => a.Id == FileId);
            var document = db.Attachements.Single(a => a.Id == FileId);             //Attachements.Single(a => a.Id == FileId);
            var FileName = "~/Repository/Attachments/" + document.Ticket.Id + "/" + document.FileUNQName;

            //var cd = new System.Net.Mime.ContentDisposition
            //{
            //    // for example foo.bak
                
            //    // always prompt the user for downloading, set to true if you want 
            //    // the browser to try to show the file inline
            //    Inline = false, 
            //};
            //Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(FileName, "file", document.FileName );
        }
    }
}
