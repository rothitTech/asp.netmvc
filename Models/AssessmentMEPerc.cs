using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class AssessmentMEPerc
    {
        public int AssessmentId { get; set; }
        public int AssessmentTypeId { get; set; }
        public string AssessmentDesc { get; set; }
        public int SchoolTermId { get; set; }
        public decimal MeetExceedPerc { get; set; }
        public decimal? MeetExceedCategory { get; set; }
        public bool IsChildExist { get; set; }
    }
}