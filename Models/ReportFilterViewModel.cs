using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class ReportFilterViewModel
    {
        public string DistrictName { get; set; }
        public string TeacherName { get; set; }
        public string SchoolYear { get; set; }
        public string ClassName { get; set; }
        public string Race { get; set; }
        public string Gender { get; set; }
        public bool? LepIndicator { get; set; }
        public bool? IepIndicator { get; set; }
        public bool? FrlIndicator { get; set; }
        public bool? Hispanic { get; set; }
        public bool IsAdmin { get; set; }
        public bool DisplayAdminFilters { get; set; }

        public string FrIndicatorDisplay
        {
            get
            {
                if (FrlIndicator == null)
                {
                    return "All";
                }
                return FrlIndicator == true ? "Yes" : "No";
            }
        }
        public string LepIndicatorDisplay
        {
            get
            {
                if (LepIndicator == null)
                {
                    return "All";
                }
                return LepIndicator == true ? "Yes" : "No";
            }
        }
        public string IepIndicatorDisplay
        {
            get
            {
                if (IepIndicator == null)
                {
                    return "All";
                }
                return IepIndicator == true ? "Yes" : "No";
            }
        }
        public string HispanicDisplay
        {
            get
            {
                if (Hispanic == null)
                {
                    return "All";
                }
                return Hispanic == true ? "Yes" : "No";
            }
        }

    }
}