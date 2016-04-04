using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EDS.Models
{
    public class AssessmentViewModel
    {
        public AssessmentViewModel()
        {
            AssessmentTypes = new List<SelectListItem>();
            SchoolYears = new List<SelectListItem>();
            SchoolTerms = new List<SelectListItem>();
            Subjects = new List<SelectListItem>();
            SLOAssessmentTemplates = new List<SelectListItem>();
            RubricAssessmentTemplates = new List<SelectListItem>();
            Assessments = new List<SelectListItem>();
        }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Score Max must be Numeric.")]
        public int? ScoreMax { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Score Min must be Numeric.")]
        public int? ScoreMin { get; set; }
        [Required(ErrorMessage = "Required")]
        public string AssessmentTypeId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SchoolYearId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SchoolTermId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SubjectId { get; set; }

        public string SLOTemplateId { get; set; }
        public string RubricTemplateId { get; set; }

        public string ParentAssessmentTypeId { get; set; }
        public IList<SelectListItem> AssessmentTypes { get; set; }
        public IList<SelectListItem> SchoolYears { get; set; }
        public IList<SelectListItem> SchoolTerms { get; set; }
        public IList<SelectListItem> Subjects { get; set; }
        public IList<SelectListItem> SLOAssessmentTemplates { get; set; }
        public IList<SelectListItem> RubricAssessmentTemplates { get; set; }
        public IList<SelectListItem> Assessments { get; set; }
    }
    public class AssessmentType
    {
        public int AssessmentTypeId { get; set; }
        public string AssessmentCode{ get; set; }
    }
}