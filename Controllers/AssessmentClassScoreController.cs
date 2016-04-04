using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using EDS.Models;
using EDS.Services;
using EDS.Helpers;

namespace EDS.Controllers
{
    [EdsAuthorize(Roles = "Data Administrator, EDS Administrator, Teacher")]
    public class AssessmentClassScoreController : Controller
    {

        dbTIREntities db;
        SiteUser siteUser;
        CommonService commonService;
        ModelServices modelServices;
        AssessmentClassScoreService assessmentClassScoreService;
        //
        // GET: /AssessmentClassScore/
        public ActionResult Index()
        {
            AssessmentClassScoreViewModel model = new AssessmentClassScoreViewModel();
            modelServices = new ModelServices();
            int schoolYearId = modelServices.SchoolYearId();
            model.SchoolYearId = Convert.ToString(schoolYearId);
            FillDropDowns(model, false);
            return View(model);
        }
        //
        // GET: /AssessmentClassScore/
        public ActionResult Search(AssessmentClassScoreViewModel model)
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                assessmentClassScoreService = new AssessmentClassScoreService(siteUser, db);

                if (assessmentClassScoreService.IsAssessmentWeightingExists(model))
                {
                    InitializeAssessmentScoreMetadata(model);
                    if (model.ScoresDetails.Count == 0)
                    {
                        ViewBag.Message = "No Record Found.";
                    }
                }
                else
                {
                    ViewBag.Message = "This combination of subject and assessment does not exist in the system for the selected school year.";
                }
                FillDropDowns(model, true);
                return View("Index", model);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [HttpPost]
        public ActionResult SaveAssessmentClassScore(List<AssessmentClassScore> model)
        {
            try
            {
                db = new dbTIREntities();
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                assessmentClassScoreService = new AssessmentClassScoreService(su, db);
                string result = assessmentClassScoreService.SaveClassAssessmentScore(model);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        private void InitializeAssessmentScoreMetadata(AssessmentClassScoreViewModel model)
        {
            int decimalPlace = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SiteDecimalPlace"].ToString());
            db = new dbTIREntities();
            SiteUser su = ((SiteUser)Session["SiteUser"]);
            ModelServices service = new ModelServices();
            assessmentClassScoreService = new AssessmentClassScoreService(su, db);
            var details = assessmentClassScoreService.GetClassAssessmentScoreData(su.Districts.First().Id, model);
            model.ScoresDetails = details;
            SetDetailReportUrlData(model);
            if (model.ScoresDetails.Count > 0)
            {
                UpdateReportTemplateConfigurations(service, model.ScoresDetails[0].ReportTemplateId);
            }
            // service.IsChildAssessmentsExists(model.AssessmentList);
        }

        private void FillDropDowns(AssessmentClassScoreViewModel model, bool isPostBack)
        {
            siteUser = ((SiteUser)Session["SiteUser"]);
            db = new dbTIREntities();
            commonService = new CommonService(siteUser, db);
            modelServices = new ModelServices();
            model.DropDown = new DropDownData();
            int schoolYearId = int.Parse(model.SchoolYearId);
            model.DistrictName = siteUser.Districts[0].Name;
            model.DistrictId = siteUser.Districts[0].Id;
            model.Assessment = commonService.GetAssessmentType();
            model.SchoolYears = commonService.GetSchoolYear();
            model.SchoolTerms = commonService.GetSchoolTerm();
            model.Subjects = commonService.GetSubjects();
            int[] userSchools = modelServices.getSchoolsByUserId(siteUser.EdsUserId).ToArray();
            model.DropDown.School = new SchoolDropDown(modelServices.GetSchoolDropDownData(siteUser.EdsUserId, schoolYearId), true, "--SELECT--", "");
            int[] schoolsTeacher = modelServices.getTeachersBySchoolsId(userSchools).ToArray();
            if (isPostBack)
            {
                model.DropDown.Teacher = new TeacherDropDown(modelServices.TeacherDropDownDataBySchoolAndYear(new int[] { int.Parse(model.SchoolId) }, schoolYearId, model.DistrictId), "--SELECT--", "");
                model.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(schoolYearId, new[] { int.Parse(model.TeacherId) }), "--SELECT--", "");
            }
            else
            {
                model.DropDown.Teacher = new TeacherDropDown(modelServices.TeacherDropDownDataBySchoolAndYear(userSchools, schoolYearId, model.DistrictId), "--SELECT--", "");
                model.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(schoolYearId, schoolsTeacher), "--SELECT--", "");
            }
        }
        private void UpdateReportTemplateConfigurations(ModelServices modelService, int reportTemplateId)
        {
            var reportTemplateConfigs = modelService.GetReportTemplateConfigurations(reportTemplateId);
            ViewBag.Projection = reportTemplateConfigs["ProjectionTitle"].Trim();
        }
        private void SetDetailReportUrlData(AssessmentClassScoreViewModel model)
        {
            ViewBag.SubjectID = model.SubjectID;
            ViewBag.SchoolYearId = model.SchoolYearId;
            ViewBag.TeacherId = model.TeacherId;
            ViewBag.ClassID = model.ClassId;
            ViewBag.AssessmentTypeId = model.AssessmentTypeId;
            ViewBag.SchoolTermId = model.SchoolTermId;
            ViewBag.SchoolId = model.SchoolId;
            ViewBag.HorizontalPageIndex = model.HorizontalPageIndex;
        }
    }
}