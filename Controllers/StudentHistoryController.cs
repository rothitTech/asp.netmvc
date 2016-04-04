using EDS.Helpers;
using EDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Controllers
{
    public class StudentHistoryController : BreadcrumNavigableController
    {
        //
        // GET: /StudentHistory/
        [EdsAuthorize]
        public ActionResult Index(int studentId, string cameFromTitle, bool showRawScale = false, int summaryCount = 0, int detailCount = 0)
        {
            try
            {
                base.SetNavigationLinksUrl();

                ModelServices modelServices = new ModelServices();
                StudentHistoryModel data = new StudentHistoryModel();

                ViewBag.StudentId = studentId;
                ViewBag.ShowRawScale = showRawScale;
                ViewBag.Count = detailCount + 1;
                ViewBag.SummaryCount = summaryCount + 1;
                SiteUser su = ((SiteUser)Session["SiteUser"]);                
                data.History = modelServices.GetStudentHistoryReport(studentId, showRawScale, su.Districts.First().Id);

                tblStudent tempStudent = modelServices.GetStudentById(studentId);
                data.CameFromTitle = cameFromTitle;
                data.Student = tempStudent.FirstName + " " + tempStudent.LastName;
                data.District = modelServices.GetDistrictName(int.Parse(tempStudent.DistrictId.ToString()));
                data.School = modelServices.GetSchoolNameByStudentId(studentId);
                return View("Index", data);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }
	}
}