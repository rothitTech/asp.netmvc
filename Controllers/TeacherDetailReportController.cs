using EDS.Helpers;
using EDS.Models;
using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDS.Constants;
using System.Net.Mime;


namespace EDS.Controllers
{
    [EdsAuthorize]
    public class TeacherDetailReportController : BreadcrumNavigableController
    {
        private void InitializeReportMetadata(TIRDetailModel model, TIRDetailReportParameter detailReportParameter)
        {
            base.SetNavigationLinksUrl();
            int decimalPlace = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SiteDecimalPlace"].ToString());

            SiteUser su = ((SiteUser)Session["SiteUser"]);
            ModelServices service = new ModelServices();

            model.SchoolYear = service.SchoolYearDescriptionByYearId(detailReportParameter.Year);

            SetDetailReportUrlData(detailReportParameter);
            UpdateReportTemplateConfigurations(service, detailReportParameter.ReportTemplateId);

            model.AssessmentList = service.GetDetailReportAssessmentList(su.Districts.First().Id, detailReportParameter);
            var list = (model.AssessmentList).Select(x => x.AssessmentDesc).ToList();

            var details = service.GetDetailReport(list, su.Districts.First().Id, detailReportParameter);
            model.details = details;
            model.Subject = service.GetSubjectDescriptionById(detailReportParameter.Subject);

            int[] studentCounts = new int[5];
            decimal[] impactSums = new decimal[5];
            decimal[] avgImpacts = new decimal[5];
            int[] countOfMeetOrExceed = new int[5];
            decimal[] percOfMeetOrExceed = new decimal[5];

            //TODO: Need to handle null check here if no data found in model.details list it will give object refrence not found error.
            studentCounts[0] = model.details.Count(d => d.ScoreData1.Impact != null);
            studentCounts[1] = model.details.Count(d => d.ScoreData2.Impact != null);
            studentCounts[2] = model.details.Count(d => d.ScoreData3.Impact != null);
            studentCounts[3] = model.details.Count(d => d.ScoreData4.Impact != null);
            studentCounts[4] = model.details.Count(d => d.ScoreData5.Impact != null);

            impactSums[0] = model.details.Sum(d => d.ScoreData1.Impact.GetValueOrDefault());
            impactSums[1] = model.details.Sum(d => d.ScoreData2.Impact.GetValueOrDefault());
            impactSums[2] = model.details.Sum(d => d.ScoreData3.Impact.GetValueOrDefault());
            impactSums[3] = model.details.Sum(d => d.ScoreData4.Impact.GetValueOrDefault());
            impactSums[4] = model.details.Sum(d => d.ScoreData5.Impact.GetValueOrDefault());

            for (int i = 0; i < studentCounts.Length; i++)
            {
                if (studentCounts[i] > 0)
                {
                    avgImpacts[i] = Math.Round(impactSums[i] / studentCounts[i], decimalPlace);
                }
            }
            countOfMeetOrExceed[0] = model.details.Where(tirDetail => tirDetail.ScoreData1.Impact >= -2).Count();
            countOfMeetOrExceed[1] = model.details.Where(tirDetail => tirDetail.ScoreData2.Impact >= -2).Count();
            countOfMeetOrExceed[2] = model.details.Where(tirDetail => tirDetail.ScoreData3.Impact >= -2).Count();
            countOfMeetOrExceed[3] = model.details.Where(tirDetail => tirDetail.ScoreData4.Impact >= -2).Count();
            countOfMeetOrExceed[4] = model.details.Where(tirDetail => tirDetail.ScoreData5.Impact >= -2).Count();

            service.IsChildAssessmentsExists(model.AssessmentList);

            for (int i = 0; i < model.AssessmentList.Count; i++)
            {
                if (studentCounts[i] > 0)
                {
                    percOfMeetOrExceed[i] = GetPerOfMeetValue(countOfMeetOrExceed[i], studentCounts[i], true);
                }
            }
            ViewBag.percOfMeetOrExceed = percOfMeetOrExceed;
            ViewBag.studentCounts = studentCounts;
            ViewBag.impactSums = impactSums;
            ViewBag.avgImpacts = avgImpacts;


            var filterParameter = new FilterParameter
            {
                ClassId = detailReportParameter.ClassId,
                Teacher = detailReportParameter.Teacher,
                Year = detailReportParameter.Year,
                Race = detailReportParameter.Race,
                Gender = detailReportParameter.Gender,
                FrlIndicator = detailReportParameter.FrlIndicator,
                IEPIndicator = detailReportParameter.IEPIndicator,
                LEPIndicator = detailReportParameter.LEPIndicator,
                Hispanic = detailReportParameter.Hispanic,
                SchoolYear = model.SchoolYear
            };

            var reportFilterViewModel = ReportsFilterHelper.PopulateReportFilterViewModel(filterParameter, service, su);
            ViewBag.ReportFilters = reportFilterViewModel;
        }
        public ActionResult AssessmentDetail(TIRDetailReportParameter detailReportParameter)
        {
            try
            {
                TIRDetailModel model = new TIRDetailModel();
                InitializeReportMetadata(model, detailReportParameter);
                model.ReportType = detailReportParameter.AssessmentTypeDesc;
                model.ReportTemplateId = detailReportParameter.ReportTemplateId;
                ViewBag.ShowPercentile = detailReportParameter.ReportTemplateId != EDS.Constants.SystemParameter.ReportTemplateType.FandPType;
                ViewBag.ShowGrowth = detailReportParameter.ReportTemplateId == EDS.Constants.SystemParameter.ReportTemplateType.FandPType;
                return View("TIRDetail", model);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }
        private decimal GetPerOfMeetValue(int countOfMeetOrExceed, int studentCount, bool viewScaledScore)
        {
            decimal percOfMeet = (decimal)countOfMeetOrExceed / studentCount * 100;
            return HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.SingleDecimalPlace, percOfMeet);
        }
        private void UpdateReportTemplateConfigurations(ModelServices modelService, int reportTemplateId)
        {
            var reportTemplateConfigs = modelService.GetReportTemplateConfigurations(reportTemplateId);
            ViewBag.Projection = reportTemplateConfigs["ProjectionTitle"].Trim();
            ViewBag.ShowToggleButton = reportTemplateConfigs["ShowToggleViewScaledScore"].Trim() == "1" ? true : false;
            ViewBag.DistPercentile = reportTemplateConfigs["DistrictPercentileTitle"].Trim();
        }
        private void SetDetailReportUrlData(TIRDetailReportParameter detailReportParameter)
        {
            ViewBag.Subject = detailReportParameter.Subject;
            ViewBag.Year = detailReportParameter.Year;
            ViewBag.AssessmentTypeDesc = detailReportParameter.AssessmentTypeDesc;
            ViewBag.Teacher = detailReportParameter.Teacher;
            ViewBag.ClassID = detailReportParameter.ClassId;
            ViewBag.Grade = detailReportParameter.Grade;
            ViewBag.ViewScaledScore = detailReportParameter.ViewScaledScore;
            ViewBag.SummaryCount = detailReportParameter.SummaryCount + 1;
            ViewBag.Race = detailReportParameter.Race;
            ViewBag.Gender = detailReportParameter.Gender;
            ViewBag.FrlIndicator = detailReportParameter.FrlIndicator;
            ViewBag.IEPIndicator = detailReportParameter.IEPIndicator;
            ViewBag.LEPIndicator = detailReportParameter.LEPIndicator;
            ViewBag.Hispanic = detailReportParameter.Hispanic;
            ViewBag.AssessmentTypeId = detailReportParameter.AssessmentTypeId;
            ViewBag.ReportTemplateId = detailReportParameter.ReportTemplateId;
        }
        public ActionResult ViewPDF(string fileName)
        {
            string filePath = System.Configuration.ConfigurationManager.AppSettings["TemplateFilePath"];
            filePath = Server.MapPath("~//" + filePath + "//" + fileName);
            Response.Clear();
            Response.ClearHeaders();
            Response.CacheControl = "Private";
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            Response.AddHeader("Cache-Control", "no-store, no-cache, must-revalidate");
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "inline; filename=" + fileName + "");
            return File(filePath, Response.ContentType);
        } 
    }
}