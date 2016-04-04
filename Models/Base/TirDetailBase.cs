using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public abstract class TirDetailBase
    {
        public int StudentId { get; set; }
        public string LocalId { get; set; }
        public string StudentName { get; set; }
        public string LastName { get; set; }
        public int AssessmentCount { get; set; }
        public int? SchoolTermId { get; set; }
    }
}
