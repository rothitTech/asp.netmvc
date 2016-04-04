using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Models
{
    public class AssessmentScoreViewModel
    {
        public AssessmentScoreViewModel()
        {
            AssessmentTypes = new List<SelectListItem>();
            SchoolYears = new List<SelectListItem>();
            SchoolTerms = new List<SelectListItem>();
           
        }
       
       
        [Display(Name = "Student Name")]
        [Required(ErrorMessage = "Required")]
        public string StudentName { get; set; }
        
        [Display(Name="District")]
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        
      
        [Display(Name = "Assessment Type")]
        [Required(ErrorMessage = "Required")]
        public string AssessmentTypeId { get; set; }
        
        [Display(Name = "School Year")]
        [Required(ErrorMessage = "Required")]
        public string SchoolYearId { get; set; }
        
        [Display(Name = "School Term")]
        [Required(ErrorMessage = "Required")]
        public string SchoolTermId { get; set; }

        public List<StudentScore> StudentScores { get; set; }
        public IList<SelectListItem> AssessmentTypes { get; set; }
        public IList<SelectListItem> SchoolYears { get; set; }
        public IList<SelectListItem> SchoolTerms { get; set; }
      
    }

    public class StudentScore
    {
        public string Subject { get; set; }
        public string LocalId { get; set; }
        public int StudentId { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        public int? Score { get; set; }
        [Display(Name = "Assessment Type")]
        public string AssessmentType { get; set; }
        [Display(Name = "School Term")]
        public string SchoolTerm { get; set; }
          [Display(Name = "School Year")]
        public string SchoolYear { get; set; }
        [Display(Name = "Assessment Desc")]
        public string AssessmentDesc { get; set; }
        public int AssessmentId { get; set; }
       
    }
}