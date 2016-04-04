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
    public class HeirarchicalTeacherDetailReportController : BreadcrumNavigableController
    {
        private void InitializeReportMetadata(HeirarchicalTIRDetailModel model, TIRDetailReportParameter detailReportParameter)
        {
            base.SetNavigationLinksUrl();

            int decimalPlace = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SiteDecimalPlace"].ToString());

            SiteUser su = ((SiteUser)Session["SiteUser"]);
            ModelServices service = new ModelServices();

            model.SchoolYear = service.SchoolYearDescriptionByYearId(detailReportParameter.Year);

            SetDetailReportUrlData(detailReportParameter);
            UpdateReportTemplateConfigurations(service, detailReportParameter.ReportTemplateId);
            ViewBag.SchoolTermList = service.GetSchoolTerms(detailReportParameter);

            var details = service.GetHeirarchicalDetailReport(su.Districts.First().Id, detailReportParameter);
            model.details = details;
            model.Subject = service.GetSubjectDescriptionById(detailReportParameter.Subject);
            model.SubjectId = detailReportParameter.Subject;
            model.SchoolYearId = detailReportParameter.Year;
            model.AssessmentTypeId = detailReportParameter.AssessmentTypeId;
            model.CameFromTitle = detailReportParameter.CameFromTitle;
            if (details.Count > 0)
            {
                model.AssessmentList = new List<AssessmentMEPerc>();
                var firstTirDetail = details[0];
                var schoolTermId = firstTirDetail.SchoolTermId;
                detailReportParameter.InputTermId = schoolTermId;
                foreach (var score in firstTirDetail.ScoreDataCollection)
                {
                    AssignMePercentForAssessment(score, service, su, detailReportParameter, ref model.AssessmentList);
                }

                service.IsChildAssessmentsExists(model.AssessmentList);

                List<int> studentCounts = studentCounts = new List<int>();
                List<decimal> impactSums = new List<decimal>();
                List<decimal> avgImpacts = new List<decimal>();
                List<int> countOfMeetOrExceed = new List<int>();

                for (int i = 0; i < model.details[0].ScoreDataCollection.Count; i++)
                {
                    studentCounts.Add(model.details.Count(d => d.ScoreDataCollection[i].Impact != null));
                    impactSums.Add(model.details.Sum(d => d.ScoreDataCollection[i].Impact.GetValueOrDefault()));
                    countOfMeetOrExceed.Add(model.details.Where(d => d.ScoreDataCollection[i].Impact >= -2).Count());
                    if (studentCounts[i] > 0)
                    {
                        avgImpacts.Add(Math.Round(impactSums[i] / studentCounts[i], decimalPlace));
                    }
                    else
                    {
                        avgImpacts.Add(0);
                    }
                }
                ViewBag.avgImpacts = avgImpacts;
            }
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

        private void AssignMePercentForAssessment(HeirarchicalScoreData scoreData, ModelServices service, SiteUser su, TIRDetailReportParameter detailReportParameter, ref List<AssessmentMEPerc> AssessmentMePercentages)
        {
            if (scoreData != null)
            {
                detailReportParameter.AssessmentTypeId = scoreData.AssessmentTypeId;
                var mePercentForAssessment = service.GetMEPercentForAssessment(su.Districts.First().Id, scoreData.AssessmentId, detailReportParameter);
                AssessmentMePercentages.Add(mePercentForAssessment);
            }
        }

        public ActionResult AssessmentDetail(TIRDetailReportParameter detailReportParameter)
        {
            try
            {
                HeirarchicalTIRDetailModel model = new HeirarchicalTIRDetailModel();
                InitializeReportMetadata(model, detailReportParameter);
                model.ReportType = detailReportParameter.AssessmentTypeDesc;
                model.ReportTemplateId = detailReportParameter.ReportTemplateId;
                ViewBag.ShowPercentile = detailReportParameter.ReportTemplateId != EDS.Constants.SystemParameter.ReportTemplateType.FandPType;
                ViewBag.ShowGrowth = detailReportParameter.ReportTemplateId == EDS.Constants.SystemParameter.ReportTemplateType.FandPType;
                return View("HeirarchicalTIRDetail", model);
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
            ViewBag.HorizontalPageIndex = detailReportParameter.HorizontalPageIndex;
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