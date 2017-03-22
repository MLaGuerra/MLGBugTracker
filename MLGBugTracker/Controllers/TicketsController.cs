using System;
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
                var tickets = db.Tickets.Where(t => t.OwnerUserId == userId)
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

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int projectId)
        {
            Ticket ticket = new Ticket();

            ticket.ProjectId = projectId;
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName");
            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");
            //ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusID = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusID,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Documents Submitter Entering Ticket is the Owner
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                ticket.TicketPriorityId = db.TicketPriorities.FirstOrDefault(n => n.Name == "Low").Id;
                ticket.TicketStatusID = 1;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusID);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
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
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusID);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusID,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusID);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
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
