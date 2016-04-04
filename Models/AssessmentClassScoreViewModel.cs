using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Models
{
    public class AssessmentClassScoreViewModel
    {
        public AssessmentClassScoreViewModel()
        {
            Assessment = new List<SelectListItem>();
            SchoolYears = new List<SelectListItem>();
            SchoolTerms = new List<SelectListItem>();
            Schools = new List<SelectListItem>();
            Subjects = new List<SelectListItem>();
        }

        [Display(Name = "District")]
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ClassId { get; set; }

        [Display(Name = "Assessment Type")]
        [Required(ErrorMessage = "Required")]
        public string AssessmentTypeId { get; set; }

        [Display(Name = "School Year")]
        [Required(ErrorMessage = "Required")]
        public string SchoolYearId { get; set; }

        [Display(Name = "School Term")]
        [Required(ErrorMessage = "Required")]
        public string SchoolTermId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SchoolId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string TeacherId { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Required")]
        public string SubjectID { get; set; }

        public DropDownData DropDown { get; set; }
        public IList<SelectListItem> Assessment { get; set; }
        public IList<SelectListItem> SchoolYears { get; set; }
        public List<SelectListItem> Subjects { get; set; }
        public IList<SelectListItem> SchoolTerms { get; set; }
        public List<SelectListItem> Schools { get; set; }
        public List<SelectListItem> Teachers { get; set; }
        public List<SelectListItem> Classes { get; set; }

        public List<AssessmentClassScoreDetail> ScoresDetails;
        public List<AssessmentClassScore> AssessmentClassScores { get; set; }
        public bool PreviousPageExist { get; set; }
        public bool NextPageExist { get; set; }
        public int HorizontalPageIndex { get; set; }

    }

    public class AssessmentClassScoreDetail : TirDetailBase
    {
        public AssessmentClassScoreDetail()
        {
            ScoreDataCollection = new List<ClassScoreData>();
        }       
        public int ReportTemplateId { get; set; }
        public List<ClassScoreData> ScoreDataCollection { get; set; }
    }
    public class AssessmentClassScore
    {
        public int AssessmentScoreId { get; set; }
        public int? Score { get; set; }
        public int? Target { get; set; }
        public int StudentId { get; set; }
        public int AssessmentId { get; set; }
        public int? GradeLevel { get; set; }
        public int SchoolId { get; set; }
    }
    public class SchoolTermAssessment
    {
        public int AssessmentId { get; set; }
        public int AssessmentTypeId { get; set; }
        public string AssessmentCode { get; set; }
        public int SchoolTermId { get; set; }
        public int? ScoreMin { get; set; }
        public int? ScoreMax { get; set; }
        public int? GradeLevel { get; set; }
    }
    public class StudentAndAssessmentType : SchoolTermAssessment
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string LastName { get; set; }
        public int ReportTemplateId { get; set; }
    }

    public class ClassScoreData : SchoolTermAssessment
    {
        public decimal? Score { get; set; }
        public decimal? Projection { get; set; }
        private string _roundingType;
        public string RoundingType
        {
            set
            {
                _roundingType = value;
            }
        }
        public string ScoreDisplay { get { return Score != null ? Score.GetValueOrDefault().ToString(_roundingType) : null; } }
        public string ProjectionDisplay { get { return Projection != null ? Projection.GetValueOrDefault().ToString(_roundingType) : null; } }
        public string AssessmentDesc { get; set; }
        public int AssessmentScoreId { get; set; }
        public int StudentId { get; set; }
    }
}