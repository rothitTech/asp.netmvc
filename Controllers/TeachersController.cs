using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDS.Models;
using System.Configuration;

namespace EDS
{
    [Authorize]
    public class TeachersController : Controller
    {
        private dbTIREntities _db;
        private ModelServices _modelServices;

        public TeachersController()
        {
            _db = new dbTIREntities();
            _modelServices = new ModelServices();

            ViewBag.SchoolYear = _modelServices.SchoolYearDescription();
        }

        // GET: /Default1/
        public ActionResult Index()
        {
            int userId = ((SiteUser)Session["SiteUser"]).EdsUserId;
            int[] userDistricts = _modelServices.GetDistrictsByUserId(userId).ToArray();

            SiteModels viewModel = new SiteModels();
            viewModel.Teachers = GetViewData(userDistricts, "", "", "");

            // Add drop down data
            viewModel.DropDown = new DropDownData();
            viewModel.DropDown.District = new DistrictDropDown(_modelServices.DistrictDropDownDataByUser(userId));
            viewModel.DropDown.School = new SchoolDropDown(_modelServices.DropDownAllOnly());

            return View(viewModel);
        }

        // GET: /Default1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblClassTeacher tblclassteacher = _db.tblClassTeachers.Find(id);
            if (tblclassteacher == null)
            {
                return HttpNotFound();
            }
            return View(tblclassteacher);
        }

        public ActionResult UpdateGrid(string hiddenDistrictFilter, string hiddenSchoolFilter)
        {
            int userId = ((SiteUser)Session["SiteUser"]).EdsUserId;
            int[] userDistricts = _modelServices.GetDistrictsByUserId(userId).ToArray();

            SiteModels viewModel = new SiteModels();
            viewModel.Teachers = GetViewData(userDistricts, hiddenDistrictFilter, hiddenSchoolFilter, "");

            // Add drop down data
            viewModel.DropDown = new DropDownData();
            viewModel.DropDown.District = new DistrictDropDown(_modelServices.DistrictDropDownDataByUser(userId));
            viewModel.DropDown.School = new SchoolDropDown(_modelServices.SchoolDropDownDataByDistrict(new int[] { Convert.ToInt32(hiddenDistrictFilter) }));

            // Reselect drop downs
            viewModel.DropDown.District.SelectedDistrict = Convert.ToInt32(hiddenDistrictFilter);
            viewModel.DropDown.School.SelectedSchool = Convert.ToInt32(hiddenSchoolFilter);

            return View("Index", viewModel);
        }

        public ActionResult Search(string firstOrLastName, string hiddenDistrictFilterSearch, string hiddenSchoolFilterSearch)
        {
            int userId = ((SiteUser)Session["SiteUser"]).EdsUserId;
            int[] userDistricts = _modelServices.GetDistrictsByUserId(userId).ToArray();

            SiteModels siteModel = new SiteModels();
            siteModel.Teachers = GetViewData(userDistricts, hiddenDistrictFilterSearch, hiddenSchoolFilterSearch, firstOrLastName);

            // Add drop down data
            siteModel.DropDown = new DropDownData();
            siteModel.DropDown.District = new DistrictDropDown(_modelServices.DistrictDropDownDataByUser(userId));
            siteModel.DropDown.School = new SchoolDropDown(_modelServices.SchoolDropDownDataByDistrict(new int[] { Convert.ToInt32(hiddenDistrictFilterSearch) }));

            // Reselect drop downs
            siteModel.DropDown.District.SelectedDistrict = Convert.ToInt32(hiddenDistrictFilterSearch);
            siteModel.DropDown.School.SelectedSchool = Convert.ToInt32(hiddenSchoolFilterSearch);

            return View("Index", siteModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region DG: Commented for March release
        // GET: /Default1/Create
        public ActionResult Create()
        {
            ViewBag.ClassId = new SelectList(_db.tblClasses, "ClassId", "ClassDesc");
            ViewBag.UserId = new SelectList(_db.tblUsers, "UserId", "UserEmail");
            return View();
        }

        // POST: /Default1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassTeacherId,ClassId,UserId,CreateDatetime,ChangeDatetime")] tblClassTeacher tblclassteacher)
        {
            if (ModelState.IsValid)
            {
                _db.tblClassTeachers.Add(tblclassteacher);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassId = new SelectList(_db.tblClasses, "ClassId", "ClassDesc", tblclassteacher.ClassId);
            ViewBag.UserId = new SelectList(_db.tblUsers, "UserId", "UserEmail", tblclassteacher.UserId);
            return View(tblclassteacher);
        }

        // GET: /Default1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblClassTeacher tblclassteacher = _db.tblClassTeachers.Find(id);
            if (tblclassteacher == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassId = new SelectList(_db.tblClasses, "ClassId", "ClassDesc", tblclassteacher.ClassId);
            ViewBag.UserId = new SelectList(_db.tblUsers, "UserId", "UserEmail", tblclassteacher.UserId);
            return View(tblclassteacher);
        }

        // POST: /Default1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassTeacherId,ClassId,UserId,CreateDatetime,ChangeDatetime")] tblClassTeacher tblclassteacher)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tblclassteacher).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassId = new SelectList(_db.tblClasses, "ClassId", "ClassDesc", tblclassteacher.ClassId);
            ViewBag.UserId = new SelectList(_db.tblUsers, "UserId", "UserEmail", tblclassteacher.UserId);
            return View(tblclassteacher);
        }

        // GET: /Default1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblClassTeacher tblclassteacher = _db.tblClassTeachers.Find(id);
            if (tblclassteacher == null)
            {
                return HttpNotFound();
            }
            return View(tblclassteacher);
        }

        // POST: /Default1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblClassTeacher tblclassteacher = _db.tblClassTeachers.Find(id);
            _db.tblClassTeachers.Remove(tblclassteacher);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        private List<Teacher> GetViewData(int[] userDistricts, string districtFilter, string schoolFilter, string searchFilter)
        {
            int userId = ((SiteUser)Session["SiteUser"]).EdsUserId;
            int schoolYear = Convert.ToInt32(ConfigurationManager.AppSettings["SchoolYear"].ToString());

            List<Teacher> model = _db.tblUsers
                .Join(_db.tblUserSchools, u => u.UserId, us => us.UserId, (u, us) => new { u, us })
                .Join(_db.tblSchools, uus => uus.us.SchoolId, s => s.SchoolId, (uus, s) => new { uus, s })

                // Always filter results by user's district
                .Where(x => userDistricts.Contains(x.s.DistrictId))
                // Always filtered by current school year
                //.Where(x => x.csys.sy.SchoolYear == schoolYear)
                .Select(y => new Teacher()
                {
                    TeacherId = y.uus.u.UserId,
                    FirstName = y.uus.u.FirstName,
                    LastName = y.uus.u.LastName,
                    SchoolDesc = y.s.SchoolDesc,
                    SchoolId = y.s.SchoolId,
                    DistrictId = y.s.DistrictId
                }).ToList();

            if (!String.IsNullOrEmpty(districtFilter) && districtFilter != "-1")
            {
                var districtsForUser = _modelServices.GetUserDistrictsFilter(userId, Convert.ToInt32(districtFilter));
                model = model.Where(x => districtsForUser.Contains(x.DistrictId)).ToList();
            }

            if (!String.IsNullOrEmpty(schoolFilter) && schoolFilter != "-1")
            {
                var schoolsForUser = _modelServices.GetUserSchoolsFilter(userId, Convert.ToInt32(schoolFilter));
                model = model.Where(x => schoolsForUser.Contains(x.SchoolId)).ToList();
            }

            if (!String.IsNullOrEmpty(searchFilter))
            {
                model = model.Where(x => x.FirstName.ToUpper().Contains(searchFilter.ToUpper()) || x.LastName.ToUpper().Contains(searchFilter.ToUpper())).ToList();
            }

            return model;
        }
    }
}
