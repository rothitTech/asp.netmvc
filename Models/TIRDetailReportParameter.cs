using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class TIRDetailReportParameter
    {
        public TIRDetailReportParameter()
        {
            Race = -1;
            Gender = -1;
            ViewScaledScore = true;
        }

        public int Subject { get; set; }
        public int Year { get; set; }
        public string AssessmentCode { get; set; }
        public int Teacher { get; set; }
        public int Grade { get; set; }
        public int ClassId { get; set; }
        public bool ViewScaledScore { get; set; }
        public int SummaryCount { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public bool? FrlIndicator { get; set; }
        public bool? IEPIndicator { get; set; }
        public bool? LEPIndicator { get; set; }
        public bool? Hispanic { get; set; }
        public int ReportTemplateId { get; set; }
        public int AssessmentTypeId { get; set; }
        public string AssessmentTypeDesc { get; set; }
        public string CameFromTitle { get; set; }
        public string ParentReferrerUrl { get; set; }
        public int? InputTermId { get; set; }
        public int? InputParentAssessmentTypeId { get; set; }
        public int HorizontalPageIndex { get; set; }
        public bool PreviousPageExist { get; set; }
        public bool NextPageExist { get; set; }
    }
}