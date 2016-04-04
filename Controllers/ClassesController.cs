using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDS.Models;
using System.Configuration;
using EntityFramework.Extensions;

using EDS.Services;
using EDS.Helpers;
using EDS.Enums;

namespace EDS
{
    [EdsAuthorize(Roles = "Data Administrator, Administrator, EDS Administrator")]
    public class ClassesController : Controller
    {
        // GET: /Classes/
        public ActionResult Index()
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                ClassService classService = new ClassService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                string currentSchoolYear = schoolService.GetCurrentSchoolYear();
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.SchoolYearId();
                ViewBag.SchoolId = modelService.DropDownDataSchool("", siteUser.EdsUserId, schoolYearId, true);
                ViewBag.SchoolYear = schoolService.DropDownDataSchoolYear(currentSchoolYear);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "CLASS");

                return View(new SiteModels()
                {
                    SchoolClasses = classService.GetViewData(currentSchoolYear, "", "")
                });
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult UpdateGrid(string hiddenSchoolFilter, string hiddenSchoolYearFilter)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                ClassService classService = new ClassService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(hiddenSchoolYearFilter));
                ViewBag.SchoolId = modelService.DropDownDataSchool(hiddenSchoolFilter, siteUser.EdsUserId, schoolYearId, true);
                ViewBag.SchoolYear = schoolService.DropDownDataSchoolYear(hiddenSchoolYearFilter);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "CLASS");
                return View("Index", new SiteModels()
                {
                    SchoolClasses = classService.GetViewData(hiddenSchoolYearFilter, hiddenSchoolFilter)
                });
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult Search(string searchClass, string hiddenSchoolFilterSearch, string hiddenSchoolYearFilterSearch)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();

                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                ClassService classService = new ClassService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(hiddenSchoolYearFilterSearch));
                ViewBag.SchoolId = modelService.DropDownDataSchool(hiddenSchoolFilterSearch, siteUser.EdsUserId, schoolYearId, true);
                ViewBag.SchoolYear = schoolService.DropDownDataSchoolYear(hiddenSchoolYearFilterSearch);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "CLASS");
                return View("Index", new SiteModels()
                {
                    SchoolClasses = classService.GetViewData(hiddenSchoolYearFilterSearch, hiddenSchoolFilterSearch, searchClass)
                });
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // GET: /Classes/Edit/5
        public ActionResult Edit(int? id, ClassWizardSteps? defaultWizardStep, bool? showTeachersForDistrict, bool? showStudentsForDistrict)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var db = new dbTIREntities();
                tblClass tblclass = db.tblClasses.Find(id);
                if (tblclass == null)
                {
                    return HttpNotFound();
                }
                PopulateViewData(tblclass, defaultWizardStep, showTeachersForDistrict, showStudentsForDistrict);
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // POST: /Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClassId,SchoolId,ClassDesc,SchoolYearId,SubjectId,Grade")] tblClass tblclass)
        {
            try
            {
                var db = new dbTIREntities();
                SiteUser siteUser = (SiteUser)Session["SiteUser"];
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                if (ModelState.IsValid)
                {
                    ClassService classService = new ClassService((SiteUser)Session["SiteUser"], db);
                    if (!(classService.IsEditClassExists(tblclass)))
                    {
                        classService.EditClass(editClass: tblclass);
                        return RedirectToAction("Edit", new { id = tblclass.ClassId, defaultWizardStep = ClassWizardSteps.Teachers });
                    }
                    else
                    {
                        PopulateViewData(tblclass, null, null, null);
                        ModelState.AddModelError("ClassDesc", "Duplicate class name - please choose a unique name.");
                        return View(tblclass);
                    }
                }
                ViewBag.SchoolYearId = modelService.GetUserSchoolYear(siteUser.EdsUserId, siteUser.Districts[0].Id, tblclass.SchoolYearId); 
                ViewBag.SchoolId = modelService.DropDownDataSchool(tblclass.SchoolId.ToString(), siteUser.EdsUserId, tblclass.SchoolYearId, true);
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

        // GET: /Classes/Create
        public ActionResult Create()
        {
            try
            {
                SiteUser siteUser = (SiteUser)Session["SiteUser"];
                var db = new dbTIREntities();
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                int defaultDistrict = siteUser.Districts[0].Id;
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.SchoolYearId();
                ViewBag.SchoolId = modelService.DropDownDataSchool("", siteUser.EdsUserId, schoolYearId, false);
                ViewBag.SchoolYearId = modelService.GetUserSchoolYear(siteUser.EdsUserId, defaultDistrict, schoolYearId);
                ViewBag.SubjectId = new SelectList(db.tblSubjects.OrderBy(x => x.SubjectDesc), "SubjectId", "SubjectDesc");
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        // POST: /Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClassId,SchoolId,ClassDesc,SchoolYearId,SubjectId,Grade,CreateDatetime,ChangeDatetime")] tblClass tblclass)
        {
            try
            {
                var db = new dbTIREntities();
                SiteUser su = (SiteUser)Session["SiteUser"];
                SchoolService schoolService = new SchoolService(su, db);
                ModelServices modelService = new ModelServices();
                if (ModelState.IsValid)
                {
                    ClassService classService = new ClassService((SiteUser)Session["SiteUser"], db);
                    if (!(classService.IsClassExists(tblclass)))
                    {
                        classService.AddClass(newClass: tblclass);
                        return RedirectToAction("Edit", new { id = tblclass.ClassId, defaultWizardStep = ClassWizardSteps.Teachers });
                    }
                    else
                    {
                        int defaultDistrict = su.Districts[0].Id;
                        ViewBag.DistrictDesc = su.Districts[0].Name;
                        ModelState.AddModelError("ClassDesc", "Duplicate class name - please choose a unique name.");
                    }
                }
                ViewBag.SchoolId = modelService.DropDownDataSchool("", su.EdsUserId, tblclass.SchoolYearId, false);
                ViewBag.SchoolYearId = modelService.GetUserSchoolYear(su.EdsUserId, su.Districts[0].Id, tblclass.SchoolYearId);
                ViewBag.SubjectId = new SelectList(db.tblSubjects.OrderBy(x => x.SubjectDesc), "SubjectId", "SubjectDesc", tblclass.SubjectId);
                return View(tblclass);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        public ActionResult UpdateTeachersInClass(string classId, List<string> teacherId)
        {
            try
            {
                ClassService classService = new ClassService((SiteUser)Session["SiteUser"], new dbTIREntities());
                return Json(classService.UpdateTeachers(Convert.ToInt32(classId), teacherId));
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        public ActionResult UpdateStudentsInClass(string classId, List<string> addedStudentIds, List<string> removedStudentIds)
        {
            try
            {
                ClassService classService = new ClassService((SiteUser)Session["SiteUser"], new dbTIREntities());
                return Json(classService.UpdateStudents(Convert.ToInt32(classId), addedStudentIds, removedStudentIds));
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        public ActionResult DeleteClass(int ClassId)
        {
            try
            {
                ClassService classService = new ClassService((SiteUser)Session["SiteUser"], new dbTIREntities());
                return Json(classService.DeleteClass(ClassId));
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
                //_db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateViewData(tblClass tblclass, ClassWizardSteps? defaultWizardStep, bool? showTeachersForDistrict, bool? showStudentsForDistrict)
        {
            ViewBag.DefaultWizardStep = defaultWizardStep;
            ViewBag.ShowTeachersForDistrict = !showTeachersForDistrict.GetValueOrDefault();
            ViewBag.ShowStudentsForDistrict = !showStudentsForDistrict.GetValueOrDefault();

            var db = new dbTIREntities();
            SiteUser siteUser = (SiteUser)Session["SiteUser"];
            SchoolService schoolService = new SchoolService(siteUser, db);
            ClassService classService = new ClassService(siteUser, db);
            ModelServices modelService = new ModelServices();

            int userAssignedDistrict = siteUser.Districts[0].Id;
            bool filterTeachersByDistrict = showTeachersForDistrict.GetValueOrDefault();
            int districtId = userAssignedDistrict;
            // Get teachers for this class
            var teachersForThisClass = classService.GetTeachersForThisClass(tblclass);

            // Get teachers not in this class
            var teachersNotForThisClass = classService.GetTeachersNotForThisClass(tblclass, filterTeachersByDistrict, districtId);

            //Get students for this class
            var studentsForThisClass = classService.GetStudentForThisClass(tblclass);

            // Get students not for this class
            var availableStudents = classService.GetStudentNotForThisClass(tblclass, districtId);

            var availableStudentsForSchool = classService.GetStudentForSchool(tblclass, districtId);

            // Add data to ViewBag for form
            ViewBag.TeachersForThisClass = new MultiSelectList(teachersForThisClass, "TeacherId", "FullName", null);
            ViewBag.AvailableTeachers = new MultiSelectList(teachersNotForThisClass, "TeacherId", "FullName", null);
            if (showStudentsForDistrict.GetValueOrDefault())
            {
                ViewBag.AvailableStudents = new MultiSelectList(availableStudents, "StudentId", "FullName", null);
            }
            else
            {
                ViewBag.AvailableStudents = new MultiSelectList(availableStudentsForSchool, "StudentId", "FullName", null);
            }
            ViewBag.StudentsForThisClass = new MultiSelectList(studentsForThisClass, "StudentId", "FullName", null);
            ViewBag.StudentsForThisClassCount = studentsForThisClass.Count();
            ViewBag.SchoolId = modelService.DropDownDataSchool(Convert.ToString(tblclass.SchoolId), siteUser.EdsUserId, tblclass.SchoolYearId, false);
            ViewBag.SchoolYearId = modelService.GetUserSchoolYear(siteUser.EdsUserId, siteUser.Districts[0].Id, tblclass.SchoolYearId); 
            ViewBag.ClassDesc = db.tblClasses.Where(x => x.ClassId == tblclass.ClassId).Select(x => x.ClassDesc).SingleOrDefault();
            ViewBag.SubjectId = new SelectList(db.tblSubjects.OrderBy(x => x.SubjectDesc), "SubjectId", "SubjectDesc", tblclass.SubjectId);
            ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "CLASS");
        }
    }
}
