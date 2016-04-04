using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class WeightingModel
    {
        public string DistrictName { get; set; }
        public string SchoolYear { get; set; }
        public List<Weighting> SummaryList { get; set; }
        public DropDownData DropDown { get; set; }
    }

    public class Weighting
    {
        public int GradeLevel { get; set; }
        public int YearId { get; set; }
        public string AssessmentCode { get; set; }
        public int SubjectId { get; set; }
        public string SubjectDesc { get; set; }
        public string AssessmentType { get; set; }
        public decimal Weight { get; set; }
    }
}