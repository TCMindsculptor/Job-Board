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
    public class UserDetailsController : Controller
    {
        private FinalEntities db = new FinalEntities();

        // GET: UserDetails
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.UserDetails.ToList());
        }

        // GET: UserDetails/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // GET: UserDetails/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            if (userDetail.UserID != User.Identity.GetUserId() && !(User.IsInRole("Admin")))
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // POST: UserDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,FirstName,LastName,ResumeFilename")] UserDetail userDetail, HttpPostedFileBase resume)
        {
            if (ModelState.IsValid)
            {

                #region User Info and File Upload

                string image = userDetail.ResumeFilename;

                //check to see if the file upload has a file
                if (resume != null)
                {
                    //yes
                    //reassign the fileName to the variable that represents the default img
                    image = resume.FileName;

                    //create a variable and retrieve the extension from the image
                    string ext = image.Substring(image.LastIndexOf("."));

                    //create a list of valid file extensions - (whitelist)
                    string[] goodExts = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };

                    //check our extension agains that list
                    if (goodExts.Contains(ext.ToLower()))
                    {
                        //as long as our extension is in that list
                        //rename the file to a unique file name and add the extension
                        image = Guid.NewGuid() + ext;

                        //remove the original file from the website AS LONG AS it is NOT our default image
                        if (userDetail.ResumeFilename != "NoResume.pdf" && userDetail.ResumeFilename != null)
                        {
                            System.IO.File.Delete(Server.MapPath("~/Content/images/Resumes/"
                                + userDetail.ResumeFilename));
                        }

                        userDetail.ResumeFilename = image;

                        //Save the new file to the website
                        resume.SaveAs(Server.MapPath("~/Content/images/Resumes/" + image));
                    }
                    //if an invalid extension is provided
                    else
                    {
                        //use orginal image
                        userDetail.ResumeFilename = userDetail.ResumeFilename;
                    }
                }
                #endregion

                db.Entry(userDetail).State = EntityState.Modified;
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(userDetail);
        }

        // GET: UserDetails/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserDetail userDetail = db.UserDetails.Find(id);
            if (userDetail == null)
            {
                return HttpNotFound();
            }
            return View(userDetail);
        }

        // POST: UserDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            UserDetail userDetail = db.UserDetails.Find(id);
            AspNetUser userAsp = db.AspNetUsers.Find(id);
            db.UserDetails.Remove(userDetail);
            db.AspNetUsers.Remove(userAsp);
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
