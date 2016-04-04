using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Constants
{

    public static class SystemParameter
    {
        public static class ReportTemplateType
        {
            public const int MapIsatType = 1;
            public const int FandPType = 2;
        } 

        public static class SummaryReportHeader
        {
            public const string MeetExceedHeaderText = "Meets/Exceeds";
            public const string ImpactScoreHeaderText = "Impact Score";
        }

        public static Dictionary<int, string> gradeDict = new Dictionary<int, string>()
                                                {
                                                    {-1, "Pre-K"},{0 , "Kindergarten"},{1 , "1"},{2 , "2"},{3 , "3"},{4 , "4"},{5 , "5"},{6 , "6"},
                                                    {7 , "7"},{8 , "8"},{9 , "9"},{10 , "10"},{11 , "11"},{12 , "12"}

                                                };
        public static class AssessmentTemplateType
        {
            public const string SLO = "SLO";
            public const string Rubric = "Rubric";
        }

        public static class ScoreTargetDefaultValue
        {
            public const int DefaultValue = -999;
        }

    }
}