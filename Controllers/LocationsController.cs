using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobBoard.DATA.EF;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using JobBoard.UI.MVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace JobBoard.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class LocationsController : Controller
    {
        private FinalEntities db = new FinalEntities();

        // GET: Locations
        public ActionResult Index()
        {
            var locations = db.Locations.Include(c => c.UserDetail);
            return View(locations.ToList());
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Locations/Create
        public async Task<ActionResult> Create()
        {

            List<ApplicationUser> managersAndAdmins = await UserManager.Users.ToListAsync();
            var userRole = db.UserDetails.ToList();

            for (int i = managersAndAdmins.Count - 1; i >= 0; i--)
            {
                for (int x = managersAndAdmins[i].Roles.Count - 1; x >= 0;  x--)
                {
                    //Only show users with roles "Admin" or "Manager"
                    if (managersAndAdmins[i].Roles.ToList()[x].RoleId == "e245ff5d-e4df-4606-b056-d3e50641d309" || managersAndAdmins[i].Roles.ToList()[x].RoleId == "bd7b11e4-df24-4fd2-acd5-60ec5eeed8ca")
                    {
                        break;
                    }
                    else if (managersAndAdmins[i].Roles.ToList()[x] == managersAndAdmins[i].Roles.ToList().Last())
                    {
                        userRole.Remove(userRole.Where(m => m.UserID == managersAndAdmins[i].Id).Single());
                    }
                }
            }
            ViewBag.ManagerID = new SelectList(userRole, "UserID", "FullName");
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationID,StoreNumber,City,State,ManagerID")] Location location)
        {
            if (!User.IsInRole("Admin"))
            {
                location.ManagerID = User.Identity.GetUserId();
            }

            if (ModelState.IsValid)
            {
                db.Locations.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ManagerID = new SelectList(db.UserDetails, "UserID", "FullName", location.ManagerID);
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            if (location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            List<ApplicationUser> managersAndAdmins = await UserManager.Users.ToListAsync();
            var userRole = db.UserDetails.ToList();

            for (int i = managersAndAdmins.Count - 1; i >= 0; i--)
            {
                for (int x = managersAndAdmins[i].Roles.Count - 1; x >= 0; x--)
                {
                    //Only show users with roles "Admin" or "Manager"
                    if (managersAndAdmins[i].Roles.ToList()[x].RoleId == "e245ff5d-e4df-4606-b056-d3e50641d309" || managersAndAdmins[i].Roles.ToList()[x].RoleId == "bd7b11e4-df24-4fd2-acd5-60ec5eeed8ca")
                    {
                        break;
                    }
                    else if (managersAndAdmins[i].Roles.ToList()[x] == managersAndAdmins[i].Roles.ToList().Last())
                    {
                        userRole.Remove(userRole.Where(m => m.UserID == managersAndAdmins[i].Id).Single());
                    }
                }
            }
            ViewBag.ManagerID = new SelectList(userRole, "UserID", "FullName", location.ManagerID);
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationID,StoreNumber,City,State,ManagerID")] Location location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ManagerID = new SelectList(db.UserDetails, "UserID", "FirstName", location.ManagerID);
            return View(location);
        }

        // GET: Locations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            if (location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);
            if (location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            db.Locations.Remove(location);
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
