using System.IO;
using System.Runtime.Serialization.Json;
using EDS.Helpers;
using EDS.Models;
using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Controllers
{
    [EdsAuthorize(Roles = "Data Administrator, EDS Administrator")]
    public class AssessmentScoreController : Controller
    {
        dbTIREntities db;
        SiteUser siteUser;
        CommonService commonService;
        AssessmentScoreService assScoreService;
        public ActionResult Index()
        {
            db = new dbTIREntities();
            siteUser = ((SiteUser)Session["SiteUser"]);
            assScoreService = new AssessmentScoreService(siteUser, db);
            commonService = new CommonService(siteUser, db);
            AssessmentScoreViewModel model = new AssessmentScoreViewModel();
            model.DistrictName = siteUser.Districts[0].Name;
            model.DistrictId = siteUser.Districts[0].Id;
            FillDropDowns(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult Search(AssessmentScoreViewModel model)
        {
            db = new dbTIREntities();
            siteUser = ((SiteUser)Session["SiteUser"]);
            assScoreService = new AssessmentScoreService(siteUser, db);
            model.DistrictName = siteUser.Districts[0].Name;
            model.DistrictId = siteUser.Districts[0].Id;
            model.StudentScores = assScoreService.GetAssessmentScoreData(model);
            FillDropDowns(model);
           TempData["AssessmentModel"] = model;
            return View("Index", model);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int studentId, string schoolYear, string schoolTerm, string assessmentType, string assessmentDesc, string localId, string subject)
        {
            try
            {
                if (TempData["AssessmentModel"] != null)
                {
                    TempData["AssessmentModel"] = TempData["AssessmentModel"];
                }
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                assScoreService = new AssessmentScoreService(siteUser, db);
                List<StudentScore> studentScores = assScoreService.GetStudentAssessmentScore(studentId, schoolYear, schoolTerm, assessmentType, assessmentDesc, localId, subject);
                StudentScore score = studentScores.FirstOrDefault();
                //StudentExt studentExt = studentService.GetStudentDetail(studentId, schoolYear);
                //PopulateViewData(studentExt);
                // ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");
                return View(score);
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
        public ActionResult Edit(
            [Bind(
                Include =
                    "Subject,LocalId,StudentId,FirstName,LastName,Score,AssessmentType,SchoolTerm,SchoolYear,AssessmentDesc,AssessmentId"
                )] StudentScore studentScore)
        {
            try
            {
                dbTIREntities db = new dbTIREntities();
                SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
                AssessmentScoreService assessmentService = new AssessmentScoreService(siteUser, db);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(siteUser.RoleDesc, "STUDENT");

                tblAssessmentScore assscore =
                    db.tblAssessmentScores.Where(
                        k => k.AssessmentId == studentScore.AssessmentId && k.StudentId == studentScore.StudentId)
                        .ToList()
                        .FirstOrDefault();
                assscore.Score = studentScore.Score;

                assessmentService.UpdateStudentScore(assscore);
                return RedirectToAction("Search", TempData["AssessmentModel"]);
                //  return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }

        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        private void FillDropDowns(AssessmentScoreViewModel model)
        {
            siteUser = ((SiteUser)Session["SiteUser"]);
            commonService = new CommonService(siteUser, db);
            assScoreService = new AssessmentScoreService(siteUser, db);
            model.AssessmentTypes = commonService.GetAssessmentType();
            model.SchoolYears = commonService.GetSchoolYear();
            model.SchoolTerms = commonService.GetSchoolTerm();
        }



    }
}