using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Models
{
    public class TIRDetailModel : TirModelBase
    {       
        public List<TIRDetail> details;
        public bool PreviousPageExist { get; set; }
        public bool NextPageExist { get; set; }
    }

    public class TIRDetail : TirDetailBase
    {
        public TIRDetail()
        {
            ScoreData1 = new ScoreData();
            ScoreData2 = new ScoreData();
            ScoreData3 = new ScoreData();
            ScoreData4 = new ScoreData();
            ScoreData5 = new ScoreData();
        }
        public string SLOFileName { get; set; }
        public string RubricFileName { get; set; }
        public ScoreData ScoreData1 { get; set; }
        public ScoreData ScoreData2 { get; set; }
        public ScoreData ScoreData3 { get; set; }
        public ScoreData ScoreData4 { get; set; }
        public ScoreData ScoreData5 { get; set; }       
    }

    public class ScoreData : TirScoreDataBase
    {
       
    }
}