using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class FilterParameter
    {
        public FilterParameter()
        {
            Race = -1;
            Gender = -1;
            ViewMeetExceedSummary = true;
        }
        public int Teacher { get; set; }
        public int School { get; set; }
        public int Year { get; set; }
        public int ClassId { get; set; }
        public int Race { get; set; }
        public int Gender { get; set; }
        public string SchoolYear { get; set; }
        public bool? FrlIndicator { get; set; }
        public bool? IEPIndicator { get; set; }
        public bool? LEPIndicator { get; set; }
        public bool? Hispanic { get; set; }
        public bool ViewMeetExceedSummary { get; set; }
    }
}