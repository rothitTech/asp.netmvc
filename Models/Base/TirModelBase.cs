using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public abstract class TirModelBase
    {
        string positiveColor = System.Configuration.ConfigurationManager.AppSettings["positiveImpactColor"].ToString();
        string neutralColor = System.Configuration.ConfigurationManager.AppSettings["neutralImpactColor"].ToString();
        string negativeColor = System.Configuration.ConfigurationManager.AppSettings["negativeImpactColor"].ToString();

        public string ReportType;
        public string Subject;
        public string DistrictName { get; set; }
        public string TeacherName { get; set; }
        public string SchoolYear { get; set; }
        public string ClassName { get; set; }
        public List<AssessmentMEPerc> AssessmentList;
        public int GetImpact(decimal? value)
        {
            int impact;

            if (value >= 2)
            {
                impact = 1;
            }
            else
            {
                if (value < -2)
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
        public string GetImpactColor(decimal? value)
        {
            string impactColor;

            if (value >= 2)
            {
                impactColor = positiveColor;
            }
            else
            {
                if (value < -2)
                {
                    impactColor = negativeColor;
                }
                else
                {
                    impactColor = neutralColor;
                }
            }

            return impactColor;
        }
        public string GetPreviousImpactArrow(int prevImpact, bool firstSummary)
        {
            string arrowName;

            if (firstSummary)
            {
                arrowName = "../content/images/right_arrow_light-gray.png";
            }
            else
            {
                if (prevImpact == 1)
                {
                    arrowName = "../content/images/right_arrow_green.png";
                }
                else
                {
                    if (prevImpact == -1)
                    {
                        arrowName = "../content/images/right_arrow_orange.png";
                    }
                    else
                    {
                        arrowName = "../content/images/right_arrow_blue.png";
                    }
                }
            }

            return arrowName;
        }
        public int ReportTemplateId { get; set; }
        public int SubjectId { get; set; }
        public int SchoolYearId { get; set; }
        public int AssessmentTypeId { get; set; }
        public string CameFromTitle { get; set; }
    }
}