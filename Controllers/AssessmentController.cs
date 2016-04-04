using EDS.Helpers;
using EDS.Models;
using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDS.Constants;

namespace EDS.Controllers
{
    [EdsAuthorize(Roles = "Data Administrator, Administrator, EDS Administrator")]
    public class AssessmentController : Controller
    {

        dbTIREntities db;
        SiteUser siteUser;
        AssessmentService assessmentService;
        CommonService commonService;
        // GET: /Assessment/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Assessment/Create
        [EdsAuthorize(Roles = "EDS Administrator")]
        public ActionResult Create()
        {
            db = new dbTIREntities();
            siteUser = ((SiteUser)Session["SiteUser"]);
            assessmentService = new AssessmentService(siteUser, db);
            AssessmentViewModel model = new AssessmentViewModel();
            FillDropDowns(model);
            return View(model);
        }

        // POST: /Assessment/Create
        [HttpPost]
        public ActionResult Create(AssessmentViewModel model)
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                assessmentService = new AssessmentService(siteUser, db);
                string result = assessmentService.CreateAssessment(model);
                ViewBag.UserMessage = result;
                AssessmentViewModel assessmentViewModel = new AssessmentViewModel();
                FillDropDowns(assessmentViewModel);
                if (result.ToLower().Contains("success"))
                {
                    ModelState.Clear();
                }
                return View(assessmentViewModel);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }
        private void FillDropDowns(AssessmentViewModel model)
        {
            siteUser = ((SiteUser)Session["SiteUser"]);
            commonService = new CommonService(siteUser, db);
            assessmentService = new AssessmentService(siteUser, db);
            model.AssessmentTypes = commonService.GetAssessmentType();
            model.Subjects = commonService.GetSubjects();
            model.SchoolYears = commonService.GetSchoolYear();
            model.SchoolTerms = commonService.GetSchoolTerm();
            model.SLOAssessmentTemplates = assessmentService.GetAssessmentTemplate(SystemParameter.AssessmentTemplateType.SLO);
            model.RubricAssessmentTemplates = assessmentService.GetAssessmentTemplate(SystemParameter.AssessmentTemplateType.Rubric);
        }
    }
}
