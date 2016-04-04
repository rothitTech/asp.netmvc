using System;
using System.Web.Mvc;
using EDS.Models;
using EDS.Services;
using EDS.Helpers;
using PagedList;
using System.Configuration;

namespace EDS.Controllers
{
    [EdsAuthorize]
    public class SupportController : Controller
    {
        //
        // GET: /Support/
        public ActionResult TechnicalSupport()
        {
            return View();
        }

        public ActionResult FeatureRequest()
        {
            return View();
        }

        public ActionResult Training()
        {
            return View();
        }

        public ActionResult Announcements(int? page)
        {
            try
            {
                int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
                int pageNumber = (page ?? 1);
                var db = new dbTIREntities();
                SiteUser su = (SiteUser)Session["SiteUser"];
                SupportService supportService = new SupportService(su, db);   
                var listData = (supportService.GetAnnouncements()).ToPagedList(pageNumber, pageSize);
                return View("Announcements", listData);
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