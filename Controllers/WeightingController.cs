using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDS.Models;
using EDS.Helpers;
using EDS.Services;

namespace EDS.Controllers
{
    [EdsAuthorize(Roles = "Data Administrator, Administrator, EDS Administrator")]
    public class WeightingController : Controller
    {
        dbTIREntities db;
        SiteUser siteUser;
        CommonService commonService;
        WeightingService weightingService;

        // GET: /Weighting/
        public ActionResult Index()
        {
            try
            {
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                ModelServices modelServices = new ModelServices();
                int userAssignedDistrict = su.Districts[0].Id;
                int schoolYearId = modelServices.SchoolYearId();

                WeightingModel data = new WeightingModel();
                data.SchoolYear = modelServices.SchoolYearDescription();
                data.SummaryList = modelServices.GetWeightingSummary(userAssignedDistrict, schoolYearId);
                data.DistrictName = modelServices.GetDistrictName(userAssignedDistrict);

                data.DropDown = new DropDownData();
                data.DropDown.Year = new YearDropDown(modelServices.SchoolYearDropDownData());
                data.DropDown.Year.SelectedYear = schoolYearId;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult UpdateGrid(int hiddenYearFilter)
        {
            try
            {
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                ModelServices modelServices = new ModelServices();

                WeightingModel data = new WeightingModel();
                int userAssignedDistrict = su.Districts[0].Id;
                data.DistrictName = modelServices.GetDistrictName(userAssignedDistrict);
                data.SchoolYear = modelServices.SchoolYearDescriptionByYearId(hiddenYearFilter);

                data.DropDown = new DropDownData();
                data.DropDown.Year = new YearDropDown(modelServices.SchoolYearDropDownData());
                data.DropDown.Year.SelectedYear = hiddenYearFilter;

                data.SummaryList = modelServices.GetWeightingSummary(userAssignedDistrict, hiddenYearFilter); ;

                return View("Index", data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        [EdsAuthorize(Roles = "EDS Administrator")]
        public ActionResult Create()
        {
            db = new dbTIREntities();
            siteUser = ((SiteUser)Session["SiteUser"]);
            commonService = new CommonService(siteUser, db);
            WeightingViewModel model = new WeightingViewModel();
            FillDropDowns(model);
            return View(model);
        }

        // POST: /Assessment/Create
        [HttpPost]
        public ActionResult Create(WeightingViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db = new dbTIREntities();
                    siteUser = ((SiteUser)Session["SiteUser"]);
                    commonService = new CommonService(siteUser, db);
                    weightingService = new WeightingService(siteUser, db);
                    bool weightingAlreadyExist = weightingService.SaveWeightingDetail(model);
                    if (!weightingAlreadyExist)
                    {
                        ViewBag.UserMessage = "Data Saved Successfully.";
                        model = new WeightingViewModel();
                        FillDropDowns(model);
                        ModelState.Clear();

                    }
                    else
                    {
                        ViewBag.UserMessage = "Data already exists.";
                        FillDropDowns(model);
                    }

                }
                catch (Exception ex)
                {
                    Logging log = new Logging();
                    log.LogException(ex);
                    return View("GeneralError");
                }
            }
            return View(model);
        }

        private void FillDropDowns(WeightingViewModel model)
        {
            siteUser = ((SiteUser)Session["SiteUser"]);
            commonService = new CommonService(siteUser, db);
            model.AssessmentTypes = commonService.GetAssessmentType();
            model.Subjects = commonService.GetSubjects();
            model.SchoolYears = commonService.GetSchoolYear();
            model.Districts = commonService.GetDistrict();
        }
    }
}
