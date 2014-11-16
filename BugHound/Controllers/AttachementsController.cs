using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BugHound.Models;
using Microsoft.AspNet.Identity;

namespace BugHound.Controllers
{
    [Authorize]
    public class AttachementsController : Controller
    {
        private BugHoundSQLEntities db = new BugHoundSQLEntities();

        // GET: Attachements
        public ActionResult Index()
        {
            var attachements = db.Attachements.Include(a => a.Ticket).Include(a => a.User);
            return View(attachements.ToList());
        }

        // GET: Attachements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachement attachement = db.Attachements.Find(id);
            if (attachement == null)
            {
                return HttpNotFound();
            }
            return View(attachement);
        }

        // GET: Attachements/Create
        public ActionResult Create(int ticketid)
        {
            //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            //ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
           
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(AttachmentViewModel attachmentVm)
        {
            if (ModelState.IsValid)
            {

                if (attachmentVm.FileObj != null && attachmentVm.FileObj.ContentLength > 0)
                {
                    var cu = User.Identity.GetUserName();
                    var usr = db.Users.Single(u => u.UserName == cu);
                    var attachment = new Attachement()
                    {
                        UserId = usr.Id,
                        DateAttached = DateTime.Now,
                        FileUNQName = Guid.NewGuid().ToString() + attachmentVm.FileObj.FileName,
                        Description = attachmentVm.Description,
                        FileName = attachmentVm.FileObj.FileName,
                        TicketId = attachmentVm.TicketId,
                        UpLoaded = true
                    };


                    var path = HttpContext.Server.MapPath("~/Repository/Attachments/" + attachmentVm.TicketId + "/");
                    var fullPath = Path.Combine(path, attachment.FileUNQName);

                    if (!Directory.Exists(path))
                    {
                        //Server.MapPath(path);
                        Directory.CreateDirectory(path);
                    }

                    attachmentVm.FileObj.SaveAs(fullPath);

                    db.Attachements.Add(attachment);

                    var tick = db.Tickets.Single(i => i.Id == attachmentVm.TicketId);
                    tick.Attachments = true;
                    tick.LastedUpdated = DateTime.Now;
                    db.Entry(tick).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                //ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachment.TicketId);
                //ViewBag.UserId = new SelectList(db.Users, "Id", "Name", attachment.UserId);
                //return View(attachment);
            }
                    return View();
            
        }

        // GET: Attachements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachement attachement = db.Attachements.Find(id);
            if (attachement == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachement.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", attachement.UserId);
            return View(attachement);
        }

        // POST: Attachements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,TicketId,FileName,UpLoaded,Description,DateAttached,FileUNQName")] Attachement attachement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attachement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", attachement.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", attachement.UserId);
            return View(attachement);
        }

        // GET: Attachements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachement attachement = db.Attachements.Find(id);
            if (attachement == null)
            {
                return HttpNotFound();
            }
            return View(attachement);
        }

        // POST: Attachements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attachement attachement = db.Attachements.Find(id);
            db.Attachements.Remove(attachement);
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
