using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Models
{
    public class KnowledgeBase
    {
        public int KnowledgeBaseId { get; set; }
        public int DistrictId { get; set; }
        public int RoleId { get; set; }
        [Required(ErrorMessage="Title is required")]
        public string Title { get; set; }
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedUserId { get; set; }
        public int ModifiedUserId { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public List<FileDetail> FileDetails { get; set; }
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }  

}

