using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class TirScoreDataBase
    {
        public decimal? Score { get; set; }
        public decimal? Projection { get; set; }
        public decimal? Impact { get; set; }
        public decimal? Percentile { get; set; }
        public decimal? Growth { get; set; }
        public int? MeetExceedValue { get; set; }
        private string _roundingType;
        private string _percentileRoundingType = "0.0";

        public string RoundingType
        {
            set
            {
                _roundingType = value;
            }
        }
        public string PercentileRoundingType
        {
            set
            {
                _percentileRoundingType = value;
            }
        }
        public string ScoreDisplay { get { return Score != null ? Score.GetValueOrDefault().ToString(_roundingType) : null; } }
        public string ProjectionDisplay { get { return Projection != null ? Projection.GetValueOrDefault().ToString(_roundingType) : null; } }
        public string ImpactDisplay { get { return Impact != null ? Impact.GetValueOrDefault().ToString(_roundingType) : null; } }
        public string PercentileDisplay { get { return Percentile != null ? Percentile.GetValueOrDefault().ToString(_percentileRoundingType) : null; } }
        public string GrowthDisplay { get { return Growth != null ? Growth.GetValueOrDefault().ToString(_percentileRoundingType) : null; } }

        public string AssessmentDesc { get; set; }
        public string AssessmentCode { get; set; }
        public int? ParentAssessmentTypeId { get; set; }
        public int AssessmentId { get; set; }
        public int AssessmentTypeId { get; set; }
    }
}