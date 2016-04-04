using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class ReportTemplateConfiguration
    {
        public int ConfigId { get; set; }
        public bool ShowToggleViewScaledScore { get; set; }
        public string ProjectionTitle { get; set; }
        public string DistrictPercentileTitle { get; set; }
    }
}