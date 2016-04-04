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

using EDS.Services;
using EDS.Helpers;

namespace EDS
{
    [EdsAuthorize]
    public class StudentsController : Controller
    {
        // GET: /Students/
        public ActionResult Index()
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                StudentService studentService = new StudentService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                string currentSchoolYear = schoolService.GetCurrentSchoolYear();
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.SchoolYearId();
                ViewBag.SchoolId = modelService.DropDownDataSchool("", siteUser.EdsUserId, schoolYearId, true);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                //ViewBag.SchoolYear = HelperService.SchoolYearDescription(db);
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(currentSchoolYear);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                ViewBag.SchoolYear = currentSchoolYear;

                return View(new SiteModels()
                {
                    Students = studentService.GetViewData(currentSchoolYear, "", "")
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
                StudentService studentService = new StudentService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();
                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(hiddenSchoolYearFilter));
                ViewBag.SchoolId = modelService.DropDownDataSchool(hiddenSchoolFilter, siteUser.EdsUserId, schoolYearId, true);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(hiddenSchoolYearFilter);

                ViewBag.SchoolYear = hiddenSchoolYearFilter;

                return View("Index", new SiteModels()
                {
                    Students = studentService.GetViewData(hiddenSchoolYearFilter, hiddenSchoolFilter)
                });
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult Search(string searchStudent, string hiddenDistrictFilterSearch, string hiddenSchoolFilterSearch, string hiddenSchoolYearFilterSearch)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                StudentService studentService = new StudentService(siteUser, db);
                SchoolService schoolService = new SchoolService(siteUser, db);
                ModelServices modelService = new ModelServices();

                ViewBag.DistrictDesc = siteUser.Districts[0].Name;
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(hiddenSchoolYearFilterSearch));
                ViewBag.SchoolId = modelService.DropDownDataSchool(hiddenSchoolFilterSearch, siteUser.EdsUserId, schoolYearId, true);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(hiddenSchoolYearFilterSearch);

                ViewBag.SchoolYear = hiddenSchoolYearFilterSearch;

                return View("Index", new SiteModels()
                {
                    Students = studentService.GetViewData(hiddenSchoolYearFilterSearch, hiddenSchoolFilterSearch, searchStudent)
                });
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
            }
            base.Dispose(disposing);
        }


        public ActionResult Create(int schoolYear)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                StudentExt studentExt = new StudentExt();
                ModelServices modelService = new ModelServices();
                studentExt.SchoolYear = schoolYear;
                PopulateViewData(studentExt);
                studentExt.SchoolYearDesc = HelperService.SchoolYearDescription(db);
                studentExt.SchoolYearId = modelService.SchoolYearId();
                return View(studentExt);

            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentId,StudentSchoolYearId,SchoolYearId,DistrictId,FirstName,MiddleName,LastName,BirthDate,Hispanic,StateId,LocalId,EnrollmentDate,IepIndicator,LepIndicator,FrlIndicator,")] StudentExt studentExt, int hdSchoolId, int? hdLineageId, int hdGradeId, int hdGenderId, int? hdRaceId, int? hdHomeLanguageId, int? hdNativeLanguageId, int schoolYear)
        {

            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                StudentService studentService = new StudentService(siteUser, db);

                if (ModelState.IsValid)
                {
                    studentExt.ServingSchoolId = hdSchoolId;
                    studentExt.LineageId = hdLineageId;
                    studentExt.NativeLanguageId = hdNativeLanguageId;
                    studentExt.HomeLanguageId = hdHomeLanguageId;
                    studentExt.RaceId = hdRaceId;
                    studentExt.GradeLevel = (short)hdGradeId;
                    studentExt.GenderId = hdGenderId;
                    studentExt.SchoolYear = schoolYear;
                    if (studentService.IsStudentExist(studentExt.StateId, null))
                    {
                        PopulateViewData(studentExt);
                        ModelState.AddModelError("StateId", "State Id already exists");
                        return View(studentExt);
                    }
                    else if (studentService.IsStudentExist(null, studentExt.LocalId))
                    {
                        PopulateViewData(studentExt);
                        ModelState.AddModelError("LocalId", "Local Id already exists");
                        return View(studentExt);
                    }
                    else
                    {
                        studentService.SaveStudents(studentExt);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int studentId, int schoolYear)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                StudentService studentService = new StudentService(siteUser, db);
                ModelServices modelService = new ModelServices();

                StudentExt studentExt = studentService.GetStudentDetail(studentId, schoolYear);
                PopulateViewData(studentExt);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                return View(studentExt);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentId,StudentSchoolYearId,SchoolYearId,DistrictId,FirstName,MiddleName,LastName,BirthDate,Hispanic,StateId,LocalId,EnrollmentDate,IepIndicator,LepIndicator,FrlIndicator,")] StudentExt studentExt, int hdSchoolId, int? hdLineageId, int hdGradeId, int hdGenderId, int? hdRaceId, int? hdHomeLanguageId, int? hdNativeLanguageId, int schoolYear)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                StudentService studentService = new StudentService(siteUser, db);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                if (ModelState.IsValid)
                {
                    studentExt.ServingSchoolId = hdSchoolId;
                    studentExt.LineageId = hdLineageId;
                    studentExt.NativeLanguageId = hdNativeLanguageId;
                    studentExt.HomeLanguageId = hdHomeLanguageId;
                    studentExt.RaceId = hdRaceId;
                    studentExt.GradeLevel = (short)hdGradeId;
                    studentExt.GenderId = hdGenderId;
                    studentExt.SchoolYear = schoolYear;
                    if (studentService.IsStudentExist(studentExt.StateId, null, studentExt.StudentId))
                    {
                        PopulateViewData(studentExt);
                        ModelState.AddModelError("StateId", "State Id already exists");
                        return View(studentExt);
                    }
                    else if (studentService.IsStudentExist(null, studentExt.LocalId, studentExt.StudentId))
                    {
                        PopulateViewData(studentExt);
                        ModelState.AddModelError("LocalId", "Local Id already exists");
                        return View(studentExt);
                    }
                    else
                    {
                        studentService.SaveStudents(studentExt);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public void PopulateViewData(StudentExt studentExt)
        {

            dbTIREntities db = new dbTIREntities();
            SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
            StudentService studentService = new StudentService(siteUser, db);
            ModelServices modelService = new ModelServices();

            studentExt.DistrictId = siteUser.Districts[0].Id;
            studentExt.DistrictDesc = siteUser.Districts[0].Name;
            studentExt.StudentClasses = studentExt.StudentId != 0 ? modelService.GetClassesByStudent(studentExt.StudentId, studentExt.SchoolYear) : null;
            studentExt.SchoolYears = studentExt.StudentId != 0 ? modelService.GetSchoolYearByStudent(studentExt.StudentId) : null;

            studentExt.DropDown = new DropDownData();
            int schoolYearId = modelService.GetSchoolYearId(studentExt.SchoolYear);
            studentExt.DropDown.School = new SchoolDropDown(modelService.GetSchoolDropDownData(siteUser.EdsUserId, schoolYearId), false);
            studentExt.DropDown.School.SelectedSchool = studentExt.ServingSchoolId;
            studentExt.DropDown.Grade = new GradeDropDown(studentService.DropDownDataForGrade());
            studentExt.DropDown.Grade.SelectedGrade = (studentExt.GradeLevel != null) ? (int)studentExt.GradeLevel : -1;
            studentExt.DropDown.Gender = new GenderDropDown(modelService.DropDownDataForGender());
            studentExt.DropDown.Gender.SelectedGender = (studentExt.GenderId != null) ? (int)studentExt.GenderId : 1;
            studentExt.DropDown.Lineage = new LineageDropDown(studentService.DropDownDataForLineage());
            studentExt.DropDown.Lineage.SelectedLineage = (studentExt.LineageId != null) ? (int)studentExt.LineageId : -1;
            studentExt.DropDown.HomeLanguage = new LanguageDropDown(studentService.DropDownDataForLanguage());
            studentExt.DropDown.HomeLanguage.SelectedLanguage = (studentExt.HomeLanguageId != null) ? (int)studentExt.HomeLanguageId : -1;
            studentExt.DropDown.NativeLanguage = new LanguageDropDown(studentService.DropDownDataForLanguage());
            studentExt.DropDown.NativeLanguage.SelectedLanguage = (studentExt.NativeLanguageId != null) ? (int)studentExt.NativeLanguageId : -1;
            studentExt.DropDown.Race = new RaceDropDown(modelService.DropDownDataForRace());
            studentExt.DropDown.Race.SelectedRace = (studentExt.RaceId != null) ? (int)studentExt.RaceId : -1;
            ViewBag.SchoolYear = studentExt.SchoolYear;
        }
    }
}