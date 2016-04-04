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
    public class SchoolsController : Controller
    {
        private dbTIREntities db = new dbTIREntities();

        // GET: /Schools/
        public ActionResult Index()
        {
            var tblschools = db.tblSchools.Include(t => t.tblAddress).Include(t => t.tblDistrict);
            return View(tblschools.ToList());
        }

        // GET: /Schools/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSchool tblschool = db.tblSchools.Find(id);
            if (tblschool == null)
            {
                return HttpNotFound();
            }
            return View(tblschool);
        }

        // GET: /Schools/Create
        public ActionResult Create()
        {
            ViewBag.AddressId = new SelectList(db.tblAddresses, "AddressId", "Address1");
            ViewBag.DistrictId = new SelectList(db.tblDistricts, "DistrictId", "DistrictDesc");
            return View();
        }

        // POST: /Schools/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="SchoolId,DistrictId,SchoolDesc,AddressId,CreateDatetime,ChangeDatetime")] tblSchool tblschool)
        {
            if (ModelState.IsValid)
            {
                db.tblSchools.Add(tblschool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AddressId = new SelectList(db.tblAddresses, "AddressId", "Address1", tblschool.AddressId);
            ViewBag.DistrictId = new SelectList(db.tblDistricts, "DistrictId", "DistrictDesc", tblschool.DistrictId);
            return View(tblschool);
        }

        // GET: /Schools/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSchool tblschool = db.tblSchools.Find(id);
            if (tblschool == null)
            {
                return HttpNotFound();
            }
            ViewBag.AddressId = new SelectList(db.tblAddresses, "AddressId", "Address1", tblschool.AddressId);
            ViewBag.DistrictId = new SelectList(db.tblDistricts, "DistrictId", "DistrictDesc", tblschool.DistrictId);
            return View(tblschool);
        }

        // POST: /Schools/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="SchoolId,DistrictId,SchoolDesc,AddressId,CreateDatetime,ChangeDatetime")] tblSchool tblschool)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblschool).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AddressId = new SelectList(db.tblAddresses, "AddressId", "Address1", tblschool.AddressId);
            ViewBag.DistrictId = new SelectList(db.tblDistricts, "DistrictId", "DistrictDesc", tblschool.DistrictId);
            return View(tblschool);
        }

        // GET: /Schools/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSchool tblschool = db.tblSchools.Find(id);
            if (tblschool == null)
            {
                return HttpNotFound();
            }
            return View(tblschool);
        }

        // POST: /Schools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSchool tblschool = db.tblSchools.Find(id);
            db.tblSchools.Remove(tblschool);
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
