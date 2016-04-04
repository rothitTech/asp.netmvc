using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class HeirarchicalTIRDetailModel : TirModelBase
    {
        public List<HeirarchicalTIRDetail> details;
    }
    public class HeirarchicalTIRDetail : TirDetailBase
    {
        public HeirarchicalTIRDetail()
        {
            ScoreDataCollection = new List<HeirarchicalScoreData>();
        }
        public string SLOFileName { get; set; }
        public string RubricFileName { get; set; }
        public List<HeirarchicalScoreData> ScoreDataCollection { get; set; }
    }
    public class HeirarchicalScoreData : TirScoreDataBase
    {

    }
}