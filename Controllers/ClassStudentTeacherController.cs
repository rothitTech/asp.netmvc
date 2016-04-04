using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDS;
using EDS.Helpers;

namespace EDS.Controllers
{
    public class ClassStudentTeacherController : Controller
    {
        private dbTIREntities db = new dbTIREntities();

        // GET: /ClassStudentTeacher/
        public ActionResult Index()
        {
            try
            {
                var tblclasses = db.tblClasses.Include(t => t.tblSchool).Include(t => t.tblSchoolYear).Include(t => t.tblSubject);
                return View(tblclasses.ToList());
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // GET: /ClassStudentTeacher/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblClass tblclass = db.tblClasses.Find(id);
                if (tblclass == null)
                {
                    return HttpNotFound();
                }
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // GET: /ClassStudentTeacher/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc");
                ViewBag.SchoolYearId = new SelectList(db.tblSchoolYears, "SchoolYearId", "SchoolYearDesc");
                ViewBag.SubjectId = new SelectList(db.tblSubjects, "SubjectId", "SubjectDesc");
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // POST: /ClassStudentTeacher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ClassId,SchoolId,ClassDesc,SchoolYearId,SubjectId,Grade,CreateDatetime,ChangeDatetime")] tblClass tblclass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.tblClasses.Add(tblclass);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblclass.SchoolId);
                ViewBag.SchoolYearId = new SelectList(db.tblSchoolYears, "SchoolYearId", "SchoolYearDesc", tblclass.SchoolYearId);
                ViewBag.SubjectId = new SelectList(db.tblSubjects, "SubjectId", "SubjectDesc", tblclass.SubjectId);
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // GET: /ClassStudentTeacher/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblClass tblclass = db.tblClasses.Find(id);
                if (tblclass == null)
                {
                    return HttpNotFound();
                }
                ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblclass.SchoolId);
                ViewBag.SchoolYearId = new SelectList(db.tblSchoolYears, "SchoolYearId", "SchoolYearDesc", tblclass.SchoolYearId);
                ViewBag.SubjectId = new SelectList(db.tblSubjects, "SubjectId", "SubjectDesc", tblclass.SubjectId);
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // POST: /ClassStudentTeacher/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ClassId,SchoolId,ClassDesc,SchoolYearId,SubjectId,Grade,CreateDatetime,ChangeDatetime")] tblClass tblclass)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(tblclass).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.SchoolId = new SelectList(db.tblSchools, "SchoolId", "SchoolDesc", tblclass.SchoolId);
                ViewBag.SchoolYearId = new SelectList(db.tblSchoolYears, "SchoolYearId", "SchoolYearDesc", tblclass.SchoolYearId);
                ViewBag.SubjectId = new SelectList(db.tblSubjects, "SubjectId", "SubjectDesc", tblclass.SubjectId);
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // GET: /ClassStudentTeacher/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblClass tblclass = db.tblClasses.Find(id);
                if (tblclass == null)
                {
                    return HttpNotFound();
                }
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // POST: /ClassStudentTeacher/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                tblClass tblclass = db.tblClasses.Find(id);
                db.tblClasses.Remove(tblclass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
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
