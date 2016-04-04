using EDS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Controllers
{
    public class JsonServiceController : Controller
    {
        private dbTIREntities _db;
        private ModelServices _modelServices;

        public JsonServiceController()
        {
            _modelServices = new ModelServices();
        }

        [HttpPost]
        public ActionResult RefreshSchoolByDistrict(int id)
        {
            return Json(_modelServices.SchoolDropDownDataByDistrict(new int[] { id }));
        }

        [HttpPost]
        public ActionResult RefreshTeacherBySchool(int schoolId, int schoolYearId)
        {
            SiteUser su = ((SiteUser)Session["SiteUser"]);
            int defaultDistrict = su.Districts[0].Id;

            if (schoolId == -1)
            {
                int[] userSchools = _modelServices.getSchoolsByUserId(su.EdsUserId).ToArray();
                return Json(_modelServices.TeacherDropDownDataBySchoolAndYear(userSchools, schoolYearId, defaultDistrict));
            }
            else
            {
                return Json(_modelServices.TeacherDropDownDataBySchoolAndYear(new int[] { schoolId }, schoolYearId, defaultDistrict));
            }
        }

        [HttpPost]
        public ActionResult RefreshClassByTeacher(int yearId, int teacherId, int schoolId)
        {
            if (teacherId == -1)
            {
                SiteUser su = ((SiteUser)Session["SiteUser"]);
                int[] schoolsTeacher = null;
                if (schoolId == -1)
                {
                    int[] userSchools = _modelServices.getSchoolsByUserId(su.EdsUserId).ToArray();
                    schoolsTeacher = _modelServices.getTeachersBySchoolsId(userSchools).ToArray();
                }
                else
                {
                    schoolsTeacher = _modelServices.getTeachersBySchoolsId(new[] { schoolId }).ToArray();
                }

                return Json(_modelServices.GetClassesByTeacher(yearId, schoolsTeacher));
            }
            else
            {
                return Json(_modelServices.GetClassesByTeacher(yearId, new[] { teacherId }));
            }
        }


        [HttpPost]
        public ActionResult GetGradeCategoryByGradeWaitingID(int id)
        {
            return Json(_modelServices.GetGradeCategoryByGradeWaitingID(id));
        }
        
        public ActionResult RefreshSchoolByYear(int schoolYearId)
        {
            SiteUser su = ((SiteUser)Session["SiteUser"]);
            int userId = su.EdsUserId;
            return Json(_modelServices.GetSchoolDropDownData(userId, schoolYearId));
        }


        [HttpPost]
        public ActionResult GetSchoolByYear(string schoolYear)
        {
            ModelServices modelService = new ModelServices();
            SiteUser siteUser = ((SiteUser)Session["SiteUser"]);
            int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(schoolYear));
            return Json(modelService.DropDownDataSchool("", siteUser.EdsUserId, schoolYearId, true), JsonRequestBehavior.AllowGet);
        }
    }
}