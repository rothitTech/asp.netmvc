using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EDS.Helpers;

namespace EDS.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult AccessDenied()
        {
            Logging log = new Logging();
            log.LogException(new UnauthorizedAccessException());
            return View("AccessDenied");
        }
       
    }
}
