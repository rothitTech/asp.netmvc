using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDS.Helpers;
using EDS.Models;
using EDS.Services;
using System.IO;
using System.Configuration;
using System.Net.Mime;

namespace EDS.Controllers
{
    [EdsAuthorize]
    public class KnowledgeBaseController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var db = new dbTIREntities();
                SiteUser su = (SiteUser)Session["SiteUser"];
                KnowledgeBaseService kbService = new KnowledgeBaseService(su, db);
                List<KnowledgeBase> KnowledgeBaseItems = new List<KnowledgeBase>();
                KnowledgeBaseItems = kbService.GetKnowledgeBase().ToList();
                ViewBag.AllowEdit = HelperService.AllowUiEdits(su.RoleDesc, "USER");
                return View(KnowledgeBaseItems);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }


        public ActionResult KnowledgeBaseDetail(int KnowledgeBaseId)
        {
            try
            {
                var db = new dbTIREntities();
                SiteUser su = (SiteUser)Session["SiteUser"];
                KnowledgeBaseService kbService = new KnowledgeBaseService(su, db);
                int DistrictId = su.Districts[0].Id;
                var listdata = kbService.GetKnowledgeBaseDetail(KnowledgeBaseId, DistrictId);
                return View(listdata);

            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult CreateKnowledgeBaseItem()
        {
            SiteUser siteUser = (SiteUser)Session["SiteUser"];
            ModelServices modelService = new ModelServices();
            ViewBag.RoleId = new SelectList(modelService.GetRolesForRole((int)(siteUser.Role)), "RoleId", "RoleDesc");
            return View();
        }

        [HttpPost]
        public ActionResult CreateKnowledgeBaseItem(KnowledgeBase model)
        {
            try
            {
                SiteUser siteUser = (SiteUser)Session["SiteUser"];
                ModelServices modelService = new ModelServices();
                List<HttpPostedFileBase> httpPostedFileBases = null;
                if (model.Files.ToList()[0] != null)
                {
                    httpPostedFileBases = model.Files.ToList();
                    var allowedFileSize = ConfigurationManager.AppSettings["FileSize"].ToString();
                    if (!DocumentManagementService.ValidateAttachmentSize(httpPostedFileBases))
                    {
                        ModelState.AddModelError("Files", "Attachment Size exceeds allowed limit of " + allowedFileSize + " MB");
                    }
                }

                if (ModelState.IsValid)
                {
                    var db = new dbTIREntities();
                    KnowledgeBaseService kbService = new KnowledgeBaseService(siteUser, db);

                    if (!kbService.IsKnowledgeBaseExists(model.Title))
                    {
                        //TODO: Till nowwe are picking first district Id. Need to refactor code when a user belongs to more than 1 district.
                        model.DistrictId = siteUser.Districts[0].Id;
                        model.RoleId = model.RoleId;
                        model.CreatedDateTime = DateTime.Now;
                        model.CreatedUserId = siteUser.EdsUserId;
                        model.FileDetails = model.Files.ToList()[0] != null ? DocumentManagementService.GetFileDetail(model.Files) : null;
                        kbService.SaveKnowledgeBase(model);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Title", "Duplicate Title - please choose a unique title.");
                    }
                }
                ViewBag.RoleId = new SelectList(modelService.GetRolesForRole((int)(siteUser.Role)), "RoleId", "RoleDesc");
                return View();
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }


        public ActionResult ViewAttachment(string physicalFileName, string fileExtension)
        {
            try
            {
                physicalFileName = physicalFileName + "" + fileExtension;
                return File(DocumentManagementService.GetFile(physicalFileName), MediaTypeNames.Application.Octet, physicalFileName);
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