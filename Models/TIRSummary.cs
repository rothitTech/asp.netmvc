using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EDS.Models
{
    public class TIRSummaryModel
    {
        public string DistrictName { get; set; }
        public string TeacherName { get; set; }
        public string SchoolYear { get; set; }
        public string ClassName { get; set; }
        public bool? LepIndicator { get; set; }
        public bool? IepIndicator { get; set; }
        public bool? FrlIndicator { get; set; }
        public bool? Hispanic { get; set; }
        public List<TIRSummary> SummaryList { get; set; }
        public DropDownData DropDown { get; set; }

    }

    public class TIRSummary
    {
        public int GradeLevel { get; set; }
        public int YearId { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public string AssessmentCode { get; set; }
        public int SubjectId { get; set; }
        public string SubjectDesc { get; set; }
        public string AssessmentTypeId { get; set; }
        public decimal Weighting { get; set; }
        public decimal? Impact { get; set; }
        public int NoOfStudent { get; set; }
        public decimal? Average { get; set; }
        public int AssessmentGradeWeightingId { get; set; }
        public decimal MeetExceedPerc { get; set; }
        public int? MeetExceedCategory { get; set; }
        public decimal? AverageOrMEValue { get; set; }
        public decimal? MeetExceedPoints { get; set; }
        public decimal? WeightedImpact { get; set; }
        public bool IsAssessmentExist { get; set; }
        public int ReportTemplateId { get; set; }
        public string AssessmentTypeDesc { get; set; }

        // This change is done to display MeetExceedPoints value = 0.67 when # of students = 0.
        // This is to support user story 212.
        public decimal? MeetExceedPointsDispaly
        {
            get
            {
                if (IsAssessmentExist && NoOfStudent == 0)
                {
                    return (decimal)0.67;
                }
                return MeetExceedPoints;
            }
        }

        public int? GetImpact(decimal? value)
        {
            int? impact = null;
            if (value != null)
            {
                if (value >= 2)
                {
                    impact = 1;
                }
                else if (value < -2)
                {
                    impact = -1;
                }
                else
                {
                    impact = 0;
                }
            }
            return impact;
        }

        public int? GetMeetExceedArrow(decimal? averageOrMEScore, int? meetExceedValue, bool ViewMeetExceedSummary)
        {
            var arrowUrl = string.Empty;
            int? meetExceedCategory;
            meetExceedCategory = ViewMeetExceedSummary ? meetExceedValue : GetImpact(averageOrMEScore);
            return meetExceedCategory;
        }

        public int? GetMeetExceedImpactCategory(decimal? ImpactScore, bool ViewMeetExceedSummary)
        {
            ModelServices modelServices = new ModelServices();
            int? meetExceedCategory;
            meetExceedCategory = ViewMeetExceedSummary ? modelServices.GetMeetExceedImpactCategory(ImpactScore) : GetImpact(ImpactScore);
            return meetExceedCategory;
        }
        /// <summary>
        /// This method will return true/false to display scaled score by default on basis of ReportTemplateId. 
        /// For ex - In case of F&P report this we are displaying scaled score only.
        /// </summary>
        /// <param name="reportTemplateId"></param>
        /// <returns></returns>
        public bool ShowScaledScoreByDefault(int reportTemplateId)
        {
            ModelServices modelService = new ModelServices();
            var reportTemplateConfigs = modelService.GetReportTemplateConfigurations(reportTemplateId);
            return reportTemplateConfigs["ShowScaledScoreByDefault"].Trim() == "1" ? true : false;
        }
    }
}
