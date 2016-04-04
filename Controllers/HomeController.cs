using EDS.Helpers;
using EDS.Models;
using EDS.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDS.Controllers
{
    [EdsAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    // Refresh the session object
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                    HelperService.SetSiteUserProfile(userManager.FindByName(User.Identity.Name), new dbTIREntities());
                }

                if (User.IsInRole("Adminstrator"))
                {
                    Session["UserIsAdmin"] = true;
                    //return list of teacher in my scope
                    return View();
                }
                else
                { Session["UserIsAdmin"] = false; }
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetAnnouncements()
        {
            try
            {
                var db = new dbTIREntities();
                SiteUser su = (SiteUser)Session["SiteUser"];
                SupportService supportService = new SupportService(su, db);

                return PartialView("_AnnouncementPartial", new SiteModels() 
                { 
                    Announcements = supportService.GetAnnouncements().ToList() 
                }); 
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