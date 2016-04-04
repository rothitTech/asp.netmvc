using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EDS
{
    public class ImportsController : Controller
    {
        private dbTIREntities db = new dbTIREntities();

        // GET: /Imports/
        public ActionResult Index()
        {
            var tblimports = db.tblImports.Include(t => t.tblImportType).Include(t => t.tblUser);
            return View(tblimports.ToList());
        }

        // GET: /Imports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImport tblimport = db.tblImports.Find(id);
            if (tblimport == null)
            {
                return HttpNotFound();
            }
            return View(tblimport);
        }

        // GET: /Imports/Create
        public ActionResult Create()
        {
            ViewBag.ImportTypeId = new SelectList(db.tblImportTypes, "ImportTypeId", "ImportTypeDesc");
            ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail");
            return View();
        }

        // POST: /Imports/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ImportId,ImportTypeId,ImportFile,UserId,ReadyToProcess,ProcesseDateTime,CreateDatetime")] tblImport tblimport, HttpPostedFileBase fileUpload)
        {
            // DG: March Release: Commented functionality for this release.
            //     Removed Session["SchoolName"]

            //if (ModelState.IsValid)
            //{
            //    if (fileUpload != null && fileUpload.ContentLength > 0)
            //    {
            //        var fileName = Path.GetFileName(fileUpload.FileName);
            //        var path = Path.Combine(Server.MapPath("~/App_Data/UpLoads/" + Session["SchoolName"] + "/"), fileName);
            //        fileUpload.SaveAs(path);
            //        tblimport.ImportFile = fileUpload.FileName;
            //        tblimport.UserId = (int)Session["EdsId"];
            //        tblimport.CreateDatetime = DateTime.Now;
            //        db.tblImports.Add(tblimport);
            //        db.SaveChanges();
            //    }

            //    return RedirectToAction("Index");
            //}

            ViewBag.ImportTypeId = new SelectList(db.tblImportTypes, "ImportTypeId", "ImportTypeDesc", tblimport.ImportTypeId);
            ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblimport.UserId);
            return View(tblimport);
        }

        // GET: /Imports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImport tblimport = db.tblImports.Find(id);
            if (tblimport == null)
            {
                return HttpNotFound();
            }
            ViewBag.ImportTypeId = new SelectList(db.tblImportTypes, "ImportTypeId", "ImportTypeDesc", tblimport.ImportTypeId);
            ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblimport.UserId);
            return View(tblimport);
        }

        // POST: /Imports/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ImportId,ImportTypeId,ImportFile,UserId,ReadyToProcess,ProcesseDateTime,CreateDatetime")] tblImport tblimport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblimport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ImportTypeId = new SelectList(db.tblImportTypes, "ImportTypeId", "ImportTypeDesc", tblimport.ImportTypeId);
            ViewBag.UserId = new SelectList(db.tblUsers, "UserId", "UserEmail", tblimport.UserId);
            return View(tblimport);
        }

        // GET: /Imports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImport tblimport = db.tblImports.Find(id);
            if (tblimport == null)
            {
                return HttpNotFound();
            }
            return View(tblimport);
        }

        // POST: /Imports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblImport tblimport = db.tblImports.Find(id);
            db.tblImports.Remove(tblimport);
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
