using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Models
{
    public class WeightingViewModel
    {

        public WeightingViewModel()
        {
            AssessmentTypes = new List<SelectListItem>();
            SchoolYears = new List<SelectListItem>();
            Subjects = new List<SelectListItem>();
            Districts = new List<SelectListItem>();
        }
     

        [Required(ErrorMessage = "Required")]
        [Range(0, 100, ErrorMessage = "Weight should be between 0 and 100")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Please enter numeric value")]
        public decimal? Weighting { get; set; }
    
        [Required(ErrorMessage = "Required")]
        public string AssessmentTypeId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SchoolYearId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string DistrictId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string SubjectId { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\-?\d+$", ErrorMessage = "Please enter numeric value.")]
        public string Grade { get; set; }

        public IList<SelectListItem> AssessmentTypes { get; set; }
        public IList<SelectListItem> SchoolYears { get; set; }
        public IList<SelectListItem> Districts { get; set; }
        public IList<SelectListItem> Subjects { get; set; }
    }
}