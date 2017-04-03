using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MLGBugTracker.Models;
using MLGBugTracker.Helpers;

namespace MLGBugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        //[Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Index()
        {
            var projs = db.Projects.ToList();
            List<ProjectPMViewModel> model = new List<ProjectPMViewModel>();

            foreach (var p in projs)
            {
                ProjectPMViewModel vm = new ProjectPMViewModel();
                vm.Project = p;
                vm.ProjectManager = p.PMID != null ? db.Users.Find(p.PMID) : null;
                model.Add(vm);
            }

            return View(model);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }
        // GET: Projects/
        public ActionResult AssignPM(int id)
        {
            AdminProjectViewModel vm = new AdminProjectViewModel();
            UserRolesHelper helper = new UserRolesHelper();

            var pms = helper.UsersInRole("ProjectManager");

            vm.PMUsers = new SelectList(pms, "Id", "FirstName");
            vm.Project = db.Projects.Find(id);

            return View(vm);
        }

        //POST: Projects/
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult AssignPM(AdminProjectViewModel adminVm)
        {
            if (ModelState.IsValid)
            {
                var prj = db.Projects.Find(adminVm.Project.Id);
                prj.PMID = adminVm.SelectedUser;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(adminVm.Project.Id);

        }

        // GET: Projects/
        public ActionResult AddDEV(int id)
        {
            ProjectDevViewModel vm = new ProjectDevViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            ProjectHelpers pHelper = new ProjectHelpers();

            var dev = helper.UsersInRole("Developer");
            var projdev = pHelper.ProjectUsersByRole(id, "Developer").Select(u => u.Id).ToArray();

            //vm.SelectedUsers = projdev;
            vm.DevUsers = new MultiSelectList(dev, "Id", "FirstName", projdev);
            vm.Project = db.Projects.Find(id);

            return View(vm);
        }

        // POST: Projects/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDEV([Bind(Include = "Project, SelectedUsers")] ProjectDevViewModel model)
        {
            ProjectHelpers helper = new ProjectHelpers();
            if (ModelState.IsValid)
            {
                var prj = db.Projects.Find(model.Project.Id);
                foreach (var usr in prj.Users)
                {
                    helper.RemoveUserFromProject(usr.Id, prj.Id);
                }

                foreach (var dev in model.SelectedUsers)
                {
                    helper.AddUserToProject(dev, model.Project.Id);
                }

                //db.SaveChanges();

                return RedirectToAction("Details", new { id = model.Project.Id });
            }
            return View(model.Project.Id);
        }


        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Name")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var projectVM = new ProjectViewModel
            {
                Project = db.Projects.Include(i => i.Users).First(i => i.Id == id)
            };

            if (projectVM == null)
            {
                return HttpNotFound();
            }

            var allProjectUsersList = db.Users.ToList();
            projectVM.AllProjectUsers = allProjectUsersList.Select(o => new SelectListItem
            {
                Text = o.UserName,
                Value = o.Id.ToString()
            });

            return View(projectVM);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(ProjectViewModel projectVM)
        {
            if (projectVM == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                //db.Entry(project).State = EntityState.Modified;

                var projectToUpdate = db.Projects
                    .Include(i => i.Users).First(i => i.Id == projectVM.Project.Id);

                if (TryUpdateModel(projectToUpdate, "Project", new string[] { "ID", "Name" }))
                {
                    var newUsers = db.Users.Where(
                        m => projectVM.SelectedProjectUsers.Contains(m.Id)).ToList();
                    var updatedUsers = new HashSet<string>(projectVM.SelectedProjectUsers);
                    foreach (ApplicationUser user in db.Users)
                    {
                        if (!updatedUsers.Contains(user.Id))
                        {
                            projectToUpdate.Users.Remove(user);
                        }
                        else
                        {
                            projectToUpdate.Users.Add((user));
                        }
                    }

                    db.Entry(projectToUpdate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(projectVM);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
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
