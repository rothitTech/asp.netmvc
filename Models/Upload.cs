using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
namespace EDS.Models
{
    public class Upload 
    {
        public int ImportId { get; set; }
        public int ImportTypeId { get; set; }
        public string ImportFile { get; set; }
        public int UserId { get; set; }
        public bool ReadyToProcess { get; set; }
        public Nullable<System.DateTime> ProcesseDateTime { get; set; }
        public System.DateTime CreateDatetime { get; set; }
        public HttpPostedFileBase uploadFile { get; set; }


        public virtual tblImportType tblImportType { get; set; }
        public virtual tblUser tblUser { get; set; }
        public virtual ICollection<tblImportAttribute> tblImportAttributes { get; set; }
        public virtual ICollection<tblImportIsat> tblImportIsats { get; set; }
        public virtual ICollection<tblImportMap> tblImportMaps { get; set; }
        public virtual ICollection<tblImportStudent> tblImportStudents { get; set; }
        public virtual ICollection<tblImportUser> tblImportUsers { get; set; }

    }
}