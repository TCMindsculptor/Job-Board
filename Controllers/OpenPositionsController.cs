using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JobBoard.DATA.EF;
using Microsoft.AspNet.Identity;

namespace JobBoard.UI.MVC.Controllers
{
    public class OpenPositionsController : Controller
    {
        private FinalEntities db = new FinalEntities();

        // GET: OpenPositions
        public ActionResult Index()
        {
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);
            return View(openPositions.ToList());
        }

        public ActionResult NewYorkCity()
        {
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);
            return View(openPositions.ToList());
        }

        public ActionResult Scranton()
        {
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);
            return View(openPositions.ToList());
        }

        public ActionResult Stamford()
        {
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);
            return View(openPositions.ToList());
        }

        public ActionResult Syracuse()
        {
            var openPositions = db.OpenPositions.Include(o => o.Location).Include(o => o.Position);
            return View(openPositions.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "User, Manager")]
        public ActionResult Apply(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            var user = User.Identity.GetUserId();

            //Delete old app
            Application oldApp = db.Applications.Where(c => c.OpenPositionID == id && c.UserID == user).SingleOrDefault();
            if (oldApp != null)
            {
                db.Applications.Remove(oldApp);
                db.SaveChanges();
            }

            Application app = new Application()
            {
                OpenPositionID = openPosition.OpenPositionID,
                UserID = user,
                ApplicationDate = DateTime.Now,
                IsDeclined = null,
                ResumeFilename = db.UserDetails.Where(c => c.UserID == user).Select(c => c.ResumeFilename).Single(),
                ManagerNotes = null
            };

            db.Applications.Add(app);
            db.SaveChanges();
            return RedirectToAction("Index", "Applications");
        }

        // GET: OpenPositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // GET: OpenPositions/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber");
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title");
            }
            else
            {
                ViewBag.LocationID = new SelectList(db.Locations.Where(c => c.ManagerID == userId), "LocationID", "StoreNumber");
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title");
            }
            return View();
        }

        // POST: OpenPositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create([Bind(Include = "OpenPositionID,PositionID,LocationID")] OpenPosition openPosition)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.OpenPositions.Add(openPosition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
                return View(openPosition);
            }
            else
            {
                ViewBag.LocationID = new SelectList(db.Locations.Where(c => c.ManagerID == userId), "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
                return View(openPosition);
            }
        }

        // GET: OpenPositions/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            if (openPosition.Location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
            }
            else
            {
                ViewBag.LocationID = new SelectList(db.Locations.Where(c => c.ManagerID == userId), "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
            }

            return View(openPosition);
        }

        // POST: OpenPositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit([Bind(Include = "OpenPositionID,PositionID,LocationID")] OpenPosition openPosition)
        {
            var userId = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Entry(openPosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
            }
            else
            {
                ViewBag.LocationID = new SelectList(db.Locations.Where(c => c.ManagerID == userId), "LocationID", "StoreNumber", openPosition.LocationID);
                ViewBag.PositionID = new SelectList(db.Positions, "PositionID", "Title", openPosition.PositionID);
            }

            return View(openPosition);
        }

        // GET: OpenPositions/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition == null)
            {
                return HttpNotFound();
            }
            if (openPosition.Location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            return View(openPosition);
        }

        // POST: OpenPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            OpenPosition openPosition = db.OpenPositions.Find(id);
            if (openPosition.Location.ManagerID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            db.OpenPositions.Remove(openPosition);
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
