using EDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDS.Helpers;
using EDS.Controllers;
using System.Configuration;
using EDS.Services;
using EDS.Constants;

namespace EDS
{
    [EdsAuthorize]
    public class TeacherSummaryReportController : BreadcrumNavigableController
    {
        private int classDefaultValue = -1;
        private readonly dbTIREntities entities = new dbTIREntities();
        // GET: /TeacherSummaryReport/
        public ActionResult Index()
        {
            SetNavigationLinksUrl();
            return View();
        }

        public ActionResult SummaryReport(bool viewMeetExceedSummary = true)
        {
            try
            {
                SetNavigationLinksUrl();
                ViewBag.SummaryLink = "SummaryReport";

                SetViewBag(viewMeetExceedSummary);
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                ModelServices modelServices = new ModelServices();
                StudentService studentService = new StudentService(su, entities);

                int defaultDistrict = su.Districts[0].Id;
                int schoolYearId = modelServices.SchoolYearId();
                int[] userSchools = modelServices.getSchoolsByUserId(su.EdsUserId).ToArray();

               

                TIRSummaryModel data = new TIRSummaryModel();
                data.SchoolYear = modelServices.SchoolYearDescription();
                data.DropDown = new DropDownData();
                data.DropDown.Year = new YearDropDown(modelServices.SchoolYearDropDownData());
                data.DropDown.Year.SelectedYear = schoolYearId;
                data.DropDown.District = new DistrictDropDown(modelServices.DistrictDropDownDataByUser(su.EdsUserId));

                data.DropDown.Race = new RaceDropDown(modelServices.DropDownDataForRace(), true);
                data.DropDown.Race.SelectedRace = -1;
                data.DropDown.Gender = new GenderDropDown(modelServices.DropDownDataForGender(), true);
                data.DropDown.Gender.SelectedGender = -1;

                data.DropDown.School = new SchoolDropDown(modelServices.GetSchoolDropDownData(su.EdsUserId, schoolYearId));
                if (su.isTeacher)
                {
                    data.DropDown.Teacher = new TeacherDropDown(
                        new List<DropDownIdName>() { new DropDownIdName() { Id = su.EdsUserId, Name = su.UserFullName } });
                    data.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(schoolYearId, new[] { su.EdsUserId }));
                }
                else // data administrator and eds administrator
                {
                    int[] schoolsTeacher = modelServices.getTeachersBySchoolsId(userSchools).ToArray();
                    data.DropDown.Teacher = new TeacherDropDown(modelServices.TeacherDropDownDataBySchoolAndYear(userSchools, schoolYearId, defaultDistrict));
                    data.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(schoolYearId, schoolsTeacher));
                }

                var filterParameter = new FilterParameter
                {
                    ClassId = classDefaultValue,
                    Teacher = su.EdsUserId,
                    School = userSchools.First(),
                    Year = schoolYearId,
                    SchoolYear = data.SchoolYear
                };
                data.SummaryList = modelServices.GetSummaryReport(filterParameter);
                var reportFilterViewModel = ReportsFilterHelper.PopulateReportFilterViewModel(filterParameter, modelServices, su);
                ViewBag.ReportFilters = reportFilterViewModel;
                return View(data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult SummaryReportById(int teacherId, bool viewMeetExceedSummary = true)
        {
            try
            {
                SetNavigationLinksUrl();
                SetViewBag(viewMeetExceedSummary);
                ViewBag.SummaryLink = "SummaryReportById";
                ViewBag.TeacherFilter = teacherId;
                bool unAuthorizedRequest = false;
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                ModelServices modelServices = new ModelServices();
                StudentService studentService = new StudentService(su, entities);
                int defaultDistrict = su.Districts[0].Id;
                int schoolYearId = modelServices.SchoolYearId();
                int[] userSchools = modelServices.getSchoolsByUserId(su.EdsUserId).ToArray();

                var dropdownTeachers = modelServices.TeacherDropDownDataBySchoolAndYear(userSchools, schoolYearId, defaultDistrict);
                unAuthorizedRequest = dropdownTeachers.Where(x => x.Id == teacherId).Count() == 0 ? true : false;

                if (unAuthorizedRequest)
                {
                    return RedirectToAction("AccessDenied", "Error");
                }

                TIRSummaryModel data = new TIRSummaryModel();
                data.SchoolYear = modelServices.SchoolYearDescription();
                data.DropDown = new DropDownData();
                data.DropDown.Year = new YearDropDown(modelServices.SchoolYearDropDownData());
                data.DropDown.Year.SelectedYear = schoolYearId;
                data.DropDown.District = new DistrictDropDown(modelServices.DistrictDropDownDataByUser(su.EdsUserId));
                data.DropDown.Teacher = new TeacherDropDown(dropdownTeachers);
                data.DropDown.Teacher.SelectedTeacher = teacherId;
                data.DropDown.Race = new RaceDropDown(modelServices.DropDownDataForRace(), true);
                data.DropDown.Gender = new GenderDropDown(modelServices.DropDownDataForGender(), true);
                data.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(schoolYearId, new[] { teacherId }));

                var filterParameter = new FilterParameter
                {
                    ClassId = classDefaultValue,
                    Teacher = teacherId,
                    School = userSchools.First(),
                    Year = schoolYearId,
                    SchoolYear = data.SchoolYear
                };
                data.SummaryList = modelServices.GetSummaryReport(filterParameter);
                data.DropDown.School = new SchoolDropDown(modelServices.GetSchoolDropDownData(su.EdsUserId, schoolYearId));
                var reportFilterViewModel = ReportsFilterHelper.PopulateReportFilterViewModel(filterParameter, modelServices, su);
                ViewBag.ReportFilters = reportFilterViewModel;
                return View("SummaryReport", data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult UpdateGrid(FilterParameter filterParameter)
        {
            try
            {
                SetNavigationLinksUrl();
                SetViewBag(filterParameter.ViewMeetExceedSummary);
                ViewBag.SummaryLink = "UpdateGrid";
                bool unAuthorizedRequest = false;
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                ModelServices modelServices = new ModelServices();
                TIRSummaryModel data = new TIRSummaryModel();

                int defaultDistrict = su.Districts[0].Id;
                data.DropDown = new DropDownData();
                ViewBag.FilterParameter = filterParameter;

                #region Initializations                
                var dropdownSchools = modelServices.GetSchoolDropDownData(su.EdsUserId, filterParameter.Year);
                var schoolCount = dropdownSchools.Where(x => x.Id == filterParameter.School).Count();
                unAuthorizedRequest = schoolCount == 0 ? true : false;
                if (unAuthorizedRequest)
                {
                    return RedirectToAction("AccessDenied", "Error");
                }

                if (!unAuthorizedRequest)
                {
                    if (su.isTeacher)
                    {
                        unAuthorizedRequest = (su.EdsUserId != filterParameter.Teacher) ? true : false;
                        if (!unAuthorizedRequest)
                            data.DropDown.Teacher = new TeacherDropDown(new List<DropDownIdName>() { new DropDownIdName() { Id = su.EdsUserId, Name = su.UserFullName } });
                    }
                    else
                    {
                        var dropdownTeachers = modelServices.TeacherDropDownDataBySchoolAndYear(new int[] { filterParameter.School }, filterParameter.Year, defaultDistrict);
                        unAuthorizedRequest = dropdownTeachers.Where(x => x.Id == filterParameter.Teacher).Count() == 0 ? true : false;

                        if (!unAuthorizedRequest)
                        {
                            data.DropDown.Teacher = new TeacherDropDown(dropdownTeachers);
                            data.DropDown.Teacher.SelectedTeacher = filterParameter.Teacher;
                        }
                    }
                }
                #endregion
                   
                //TODO: need to refactor below code if possible.
                ViewBag.TeacherFilter = filterParameter.Teacher;
                ViewBag.SchoolFilter = filterParameter.School;
                ViewBag.YearFilter = filterParameter.Year;
                ViewBag.ClassFilter = filterParameter.ClassId;
                ViewBag.Race = filterParameter.Race;
                ViewBag.Gender = filterParameter.Gender;
                ViewBag.FrlIndicator = filterParameter.FrlIndicator;
                ViewBag.IEPIndicator = filterParameter.IEPIndicator;
                ViewBag.LEPIndicator = filterParameter.LEPIndicator;
                ViewBag.Hispanic = filterParameter.Hispanic;


                #region Init Filter Parameters
                data.SchoolYear = modelServices.SchoolYearDescriptionByYearId(filterParameter.Year);
                data.DropDown.Year = new YearDropDown(modelServices.SchoolYearDropDownData());
                data.DropDown.Year.SelectedYear = filterParameter.Year;
                data.DropDown.District = new DistrictDropDown(modelServices.DistrictDropDownDataByUser(su.EdsUserId));
                data.DropDown.School = new SchoolDropDown(dropdownSchools);
                data.DropDown.School.SelectedSchool = filterParameter.School;
                data.SummaryList = modelServices.GetSummaryReport(filterParameter);
             
                data.DropDown.SchoolClass = new ClassDropDown(modelServices.GetClassesByTeacher(filterParameter.Year, new[] { filterParameter.Teacher }));

                data.DropDown.Race = new RaceDropDown(modelServices.DropDownDataForRace(), true);
                data.DropDown.Race.SelectedRace = filterParameter.Race;
                data.DropDown.Gender = new GenderDropDown(modelServices.DropDownDataForGender(), true);
                data.DropDown.Gender.SelectedGender = filterParameter.Gender;
                data.Hispanic = filterParameter.Hispanic;
                data.IepIndicator = filterParameter.IEPIndicator;
                data.LepIndicator = filterParameter.LEPIndicator;
                data.FrlIndicator = filterParameter.FrlIndicator;

                data.DropDown.SchoolClass.SelectedClass = filterParameter.ClassId;
                #endregion

                filterParameter.SchoolYear = data.SchoolYear;
                var reportFilterViewModel = ReportsFilterHelper.PopulateReportFilterViewModel(filterParameter, modelServices, su);
                ViewBag.ReportFilters = reportFilterViewModel;
                return View("SummaryReport", data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        private void SetViewBag(bool ViewMeetExceedSummary)
        {
            ViewBag.ViewMeetExceedandImpact = (ViewMeetExceedSummary) ? SystemParameter.SummaryReportHeader.MeetExceedHeaderText : SystemParameter.SummaryReportHeader.ImpactScoreHeaderText;
            ViewBag.PercentSign = (ViewMeetExceedSummary) ? "%" : string.Empty;
            ViewBag.ViewMeetExceedSummary = ViewMeetExceedSummary;
        }
    }
}