using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class StudentHistoryModel
    {
        public string District { get; set; }
        public string School { get; set; }
        public string Student { get; set; }
        public string CameFromTitle { get; set; }
        public List<StudentHistory> History {get;set;}
        public DropDownData DropDown { get; set; }
    }

    public class StudentHistory
    {
        public string Subject {get;set;}
        public string AssessmentCode { get; set; }
        public string AssessmentDesc { get; set; }
        public decimal? Score { get; set; }
        public decimal? Growth { get; set; }
        public decimal? NationalPct { get; set; }
        public decimal? DistrictPct { get; set; }
        public decimal? Projection { get; set; }
        public decimal? Impact { get; set; }
        public int? MeetExceedValue { get; set; }
        public int? ParentAssessmentTypeId { get; set; }
    }
}