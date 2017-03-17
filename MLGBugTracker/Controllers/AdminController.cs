using MLGBugTracker.Helpers;
using MLGBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MLGBugTracker.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserRolesHelper helper = new UserRolesHelper();

        // GET: Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<UsersViewModel> users = new List<UsersViewModel>();
            var dbUsers = db.Users.ToList();

            foreach(var usr in dbUsers)
            {
                UsersViewModel vm = new UsersViewModel();
                vm.User = usr;
                vm.Roles = helper.ListUserRoles(usr.Id).ToList();
                users.Add(vm);
            }

            return View(users);
        }

        public ActionResult AddUserRole()
        {
            ViewBag.Users = new SelectList(db.Users,"Id", "FirstName");
            ViewBag.Roles = new SelectList(db.Roles,"Name", "Name");

            return View();
        }
        // POST: Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserRole(string Users, string Roles)
        {
            helper.AddUserToRole(Users, Roles);

            return RedirectToAction("Index");
        }
    }
}