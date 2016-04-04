using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDS;
using EDS.Models;

namespace EDS.Views
{
    [HandleError(ExceptionType = typeof(Exception), View = "GeneralError")]
    public class SchoolTeacherController : Controller
    {
        private dbTIREntities db = new dbTIREntities();
        private ModelServices modelServices = new ModelServices();
        // GET: /SchoolTeacher/
        public ActionResult Index()
        {
            try
            {
                // TODO: DG: This controller is not used for March release.
                //           Need to select the correct school id.
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                List<UserSchool> userSchool = ((SiteUser)Session["SiteUser"]).Schools.Take(1).ToList();
                int schoolId = userSchool[0].Id;

                List<int> teacherIds = modelServices.GetSchoolTeacherIds(schoolId);
                //var tblschooluserroles = (from s in db.tblSchoolUserRoles.Include(t => t.tblRole).Include(t => t.tblSchool).Include(t => t.tblUser)
                //                          where (teacherIds.Contains(s.UserId))
                //                          select s);

                //return View(tblschooluserroles.ToList());
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                throw;
            }
        }

        // GET: /SchoolTeacher/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //tblSchoolUserRole tblschooluserrole = db.tblSchoolUserRoles.Find(id);
            //if (tblschooluserrole == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(tblschooluserrole);
            return View();
        }

        // GET: /SchoolTeacher/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.RoleId = new SelectList(db.tblRoles, "RoleId", "RoleDesc");
                ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc");
                ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "FirstName");
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                throw;
            }
        }

        // POST: /SchoolTeacher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="SchoolRoleId,SchoolId,UserId,RoleId,CreateDatetime,ChangeDatetime")] tblSchoolUserRole tblschooluserrole)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tblSchoolUserRoles.Add(tblschooluserrole);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.RoleId = new SelectList(db.tblRoles, "RoleId", "RoleDesc", tblschooluserrole.RoleId);
        //    ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblschooluserrole.SchoolId);
        //    ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblschooluserrole.UserId);
        //    return View(tblschooluserrole);
        //}

        // GET: /SchoolTeacher/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //tblSchoolUserRole tblschooluserrole = db.tblSchoolUserRoles.Find(id);
            //if (tblschooluserrole == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.RoleId = new SelectList(db.tblRoles, "RoleId", "RoleDesc", tblschooluserrole.RoleId);
            //ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblschooluserrole.SchoolId);
            //ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblschooluserrole.UserId);
            //return View(tblschooluserrole);
            return View();
        }

        // POST: /SchoolTeacher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="SchoolRoleId,SchoolId,UserId,RoleId,CreateDatetime,ChangeDatetime")] tblSchoolUserRole tblschooluserrole)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tblschooluserrole).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.RoleId = new SelectList(db.tblRoles, "RoleId", "RoleDesc", tblschooluserrole.RoleId);
        //    ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblschooluserrole.SchoolId);
        //    ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblschooluserrole.UserId);
        //    return View(tblschooluserrole);
        //}

        // GET: /SchoolTeacher/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    tblSchoolUserRole tblschooluserrole = db.tblSchoolUserRoles.Find(id);
        //    if (tblschooluserrole == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tblschooluserrole);
        //}

        // POST: /SchoolTeacher/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    tblSchoolUserRole tblschooluserrole = db.tblSchoolUserRoles.Find(id);
        //    db.tblSchoolUserRoles.Remove(tblschooluserrole);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
