﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MLGBugTracker.Models;
using Microsoft.AspNet.Identity;
using MLGBugTracker.Helpers;
using System.Threading.Tasks;
using System.IO;

namespace MLGBugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            TicketIndexViewModel model = new TicketIndexViewModel();
            UserRolesHelper helper = new UserRolesHelper();

            if (User.IsInRole("Admin"))
            {
                var tickets = db.Tickets.Include(t => t.AssignedToUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.Project)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType);
                model.AdminTickets = tickets.ToList();
            }

            if (User.IsInRole("ProjectManager"))
            {
                var proj = db.Projects;
                var myproj = proj.Where(p => p.PMID == userId);
                var tkts = myproj.SelectMany(t => t.Tickets);

                tkts.Include(t => t.AssignedToUser)
                                         .Include(t => t.OwnerUser)
                                         .Include(t => t.Project)
                                         .Include(t => t.TicketPriority)
                                         .Include(t => t.TicketStatus)
                                         .Include(t => t.TicketType);
                model.PMTickets = tkts.ToList();
            }

            if (User.IsInRole("Developer"))
            {
                var tickets = db.Tickets.Where(t => t.AssignedToUserId == userId)
                                        .Include(t => t.AssignedToUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.Project)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType);
                model.DevTickets = tickets.ToList();

            }

            if (User.IsInRole("Submitter"))
            {
                var tickets = db.Tickets.Where(t => t.OwnerUserId == userId)
                                        .Include(t => t.AssignedToUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.Project)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType);
                model.SubTickets = tickets.ToList();
            }

            return View(model);
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
            return View(ticket);
        }
        // GET: Tickets/AssignDev
        public ActionResult AssignDev(int ticketId)
        {
            AssignDevViewModel vm = new AssignDevViewModel();
            ProjectHelpers helper = new ProjectHelpers();

            var tkt = db.Tickets.Find(ticketId);
            var dev = helper.ProjectUsersByRole(tkt.ProjectId, "Developer");

            vm.Developers = new SelectList(dev, "Id", "FirstName");
            vm.Ticket = tkt;

            return View(vm);
        }


        //POST: Tickets/AssignDev
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignDev(AssignDevViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tkt = db.Tickets.Find(model.Ticket.Id);
                tkt.AssignedToUserId = model.SelectedUser;
                db.SaveChanges();

                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();

                msg.Body = "You've been assigned a new development ticket.";
                msg.Destination = db.Users.Find(model.SelectedUser).Email;
                msg.Subject = "New Ticket Assignment";

                await ems.SendMailAsync(msg);

                return RedirectToAction("Details", "Project", new { id = tkt.ProjectId });
            }
            return View(model.Ticket.Id);
        }

        //POST; Add Attachment
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult AddAttachment(HttpPostedFileBase image, int ticketId)
        {
            TicketAttachment ta = new TicketAttachment();
            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                ta.FileUrl = "/Uploads/" + fileName;
            }

            ta.TicketId = ticketId;
            ta.Created = DateTimeOffset.Now;
            db.TicketAttachments.Add(ta);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = ta.TicketId });
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter, Admin")]
        public ActionResult Create(int projectId)
        {
            Ticket ticket = new Ticket();

            ticket.ProjectId = projectId;
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter, Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusID,OwnerUserId,AssignedToUserId")] Ticket ticket, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                //Documents Submitter Entering Ticket is the Owner
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                ticket.TicketPriorityId = db.TicketPriorities.FirstOrDefault(n => n.Name == "Low").Id;
                ticket.TicketStatusId = 1;

                db.Tickets.Add(ticket);
                db.SaveChanges();

                return RedirectToAction("Details", "Project", new { id = ticket.ProjectId });
            }

            Ticket tkt = new Ticket();
            ticket.ProjectId = ticket.ProjectId;
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return HttpNotFound();
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

            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusID,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                Ticket oldTicket = db.Tickets.AsNoTracking().Where(m => m.Id == ticket.Id).FirstOrDefault();

                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");

                //Add History Record
                if (oldTicket.Title != ticket.Title)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "Title";
                    tHistory.OldValue = oldTicket.Title;
                    tHistory.NewValue = ticket.Title;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();

                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                }

                if (oldTicket.Description != ticket.Description)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "Description";
                    tHistory.OldValue = oldTicket.Description;
                    tHistory.NewValue = ticket.Description;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();

                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                }

                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "TicketType";
                    tHistory.OldValue = db.TicketTypes.Find(oldTicket.TicketTypeId).Name;
                    tHistory.NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();

                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                }

                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "TicketPriority";
                    tHistory.OldValue = db.TicketPriorities.Find(oldTicket.TicketPriorityId).Name;
                    tHistory.NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();

                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    TicketHistory tHistory = new TicketHistory();
                    tHistory.Changed = DateTimeOffset.Now;
                    tHistory.Property = "TicketStatus";
                    tHistory.OldValue = db.TicketStatuses.Find(oldTicket.TicketStatusId).Name;
                    tHistory.NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name;
                    tHistory.TicketId = ticket.Id;
                    tHistory.UserId = User.Identity.GetUserId();

                    db.TicketHistories.Add(tHistory);
                    db.SaveChanges();
                }

                return RedirectToAction("Details", "Project", new { id = ticket.ProjectId });
            }
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }
        //POST: History Record
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "TicketId, UserId, Comment")]TicketComment tcomm)
        {
            if (ModelState.IsValid)
            {
                tcomm.Created = DateTimeOffset.Now;
                db.TicketComments.Add(tcomm);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = tcomm.TicketId });

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
