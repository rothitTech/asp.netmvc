using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EDS
{
    public class SchoolAdminViewController : Controller
    {
        private dbTIREntities db = new dbTIREntities();

        // GET: /SchoolAdminView/
        public ActionResult Index()
        {
            var xc = db.SchoolTeachers.ToList();
            return View(db.SchoolTeachers.ToList());
        }

        // GET: /SchoolAdminView/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolTeacher schoolteacher = db.SchoolTeachers.Find(id);
            if (schoolteacher == null)
            {
                return HttpNotFound();
            }
            return View(schoolteacher);
        }

        // GET: /SchoolAdminView/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /SchoolAdminView/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SchoolDesc,FirstName,LastName,SchoolId,RoleId,UserEmail,UserId")] SchoolTeacher schoolteacher)
        {
            if (ModelState.IsValid)
            {
                db.SchoolTeachers.Add(schoolteacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(schoolteacher);
        }

        // GET: /SchoolAdminView/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolTeacher schoolteacher = db.SchoolTeachers.Find(id);
            if (schoolteacher == null)
            {
                return HttpNotFound();
            }
            return View(schoolteacher);
        }

        // POST: /SchoolAdminView/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="SchoolDesc,FirstName,LastName,SchoolId,RoleId,UserEmail,UserId")] SchoolTeacher schoolteacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(schoolteacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(schoolteacher);
        }

        // GET: /SchoolAdminView/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SchoolTeacher schoolteacher = db.SchoolTeachers.Find(id);
            if (schoolteacher == null)
            {
                return HttpNotFound();
            }
            return View(schoolteacher);
        }

        // POST: /SchoolAdminView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            SchoolTeacher schoolteacher = db.SchoolTeachers.Find(id);
            db.SchoolTeachers.Remove(schoolteacher);
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
