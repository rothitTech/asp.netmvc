using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EDS;
using EDS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using EntityFramework.Extensions;

using EDS.Services;
using EDS.Helpers;

namespace EDS.Controllers
{
    [EdsAuthorize(Roles = "Data Administrator, Administrator, EDS Administrator")]
    public class UserController : Controller
    {
        SiteUser siteUser;
        dbTIREntities db;
        UserService userService;
        SchoolService schoolService;
        ModelServices modelService;

        public ActionResult Index()
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                userService = new UserService(siteUser, db);
                schoolService = new SchoolService(siteUser, db);
                modelService = new ModelServices();
                int schoolYearId = modelService.SchoolYearId();
                string currentSchoolYear = schoolService.GetCurrentSchoolYear();
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(currentSchoolYear);
                FillViewBagValues(siteUser.Districts[0].Name, string.Empty, siteUser.RoleDesc, schoolYearId);
                return View(userService.GetViewData(currentSchoolYear, "", ""));
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
                //throw;
            }
        }

        public ActionResult Search(string search, string schoolFilterSearch, string schoolYearSearch)
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                userService = new UserService(siteUser, db);
                schoolService = new SchoolService(siteUser, db);
                modelService = new ModelServices();
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(schoolYearSearch);
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(schoolYearSearch));
                FillViewBagValues(siteUser.Districts[0].Name, schoolFilterSearch, siteUser.RoleDesc, schoolYearId);
                return View("Index", userService.GetViewData(schoolYearSearch, schoolFilterSearch, search));
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult UpdateGrid(string hiddenSchoolFilter, string schoolYearFilter)
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                userService = new UserService(siteUser, db);
                schoolService = new SchoolService(siteUser, db);
                modelService = new ModelServices();
                ViewBag.SchoolYearList = schoolService.DropDownDataSchoolYear(schoolYearFilter);
                int schoolYearId = modelService.GetSchoolYearId(Convert.ToInt32(schoolYearFilter));
                FillViewBagValues(siteUser.Districts[0].Name, hiddenSchoolFilter, siteUser.RoleDesc, schoolYearId);
                return View("Index", userService.GetViewData(schoolYearFilter, hiddenSchoolFilter, ""));
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }



        // GET: /User/Create
        public ActionResult Create()
        {
            try
            {
                db = new dbTIREntities();
                siteUser = ((SiteUser)Session["SiteUser"]);
                modelService = new ModelServices();
                schoolService = new SchoolService(siteUser, db);
                userService = new UserService(siteUser, db);

                int userAssignedDistrict = siteUser.Districts[0].Id;
                int schoolYearId = modelService.SchoolYearId();

                tblUserExt userExtended = new tblUserExt();
                userExtended.SchoolYearId = schoolYearId;
                userExtended.Schools = userService.GetSchoolWithCheckBoxes(userExtended);
                ViewBag.RoleId = new SelectList(modelService.GetRolesForRole((int)(siteUser.Role)), "RoleId", "RoleDesc");
                FillViewBagValues(siteUser.Districts[0].Name, string.Empty, siteUser.RoleDesc, schoolYearId);
                return View(userExtended);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(tblUserExt tblUserExtended)
        {
            try
            {
                db = new dbTIREntities();
                modelService = new ModelServices();
                siteUser = ((SiteUser)Session["SiteUser"]);
                userService = new UserService(siteUser, db);
                schoolService = new SchoolService(siteUser, db);

                int userAssignedDistrict = siteUser.Districts[0].Id;
                string currentSchoolYear = schoolService.GetCurrentSchoolYear();

                if (ModelState.IsValid)
                {
                    if (tblUserExtended.SelectedSchools != null && tblUserExtended.SelectedSchools.Count() > 0)
                    {
                        var context = new Models.ApplicationDbContext();
                        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                        // 1. Create ASPNET user
                        string userName = tblUserExtended.UserName;
                        string password = tblUserExtended.Password;

                        var isPasswordValid = password != null && password.Length >= 6 ? true : false;
                        var isUserNameExist = userManager.FindByName(userName);
                        bool isEmailAddressExist = db.tblUsers.Where(x => x.UserEmail == tblUserExtended.UserEmail).Count() > 0 ? true : false;
                        bool isStateIdExist = db.tblUsers.Where(x => x.StateId == tblUserExtended.StateId).Count() > 0 ? true : false;

                        if ((isUserNameExist == null) && (!isEmailAddressExist) && (!isStateIdExist) && (isPasswordValid))
                        {
                            var user = new ApplicationUser() { UserName = userName };
                            var result = await userManager.CreateAsync(user, password);
                            if (result.Succeeded)
                            {
                                // 2. Create EDS user
                                ApplicationUser newAspNetUser = userManager.FindByName(userName);
                                if (newAspNetUser != null)
                                {
                                    userService.CreateEdsUser(newAspNetUser.Id, tblUserExtended);
                                }
                            }
                            else
                            {
                                throw new Exception(String.Format("ERROR: {0}", result.Errors));
                            }
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            if (isUserNameExist != null)
                                ModelState.AddModelError("UserName", "Duplicate name - please choose a unique name.");
                            if (isEmailAddressExist)
                                ModelState.AddModelError("UserEmail", "Duplicate email - please choose a unique email.");
                            if (isStateIdExist)
                                ModelState.AddModelError("StateId", "Duplicate state id - please choose a unique state.");
                            if (!isPasswordValid)
                                ModelState.AddModelError("Password", "Please enter password at least 6 characters.");
                        }
                    }
                    else
                    {
                        ViewBag.SchoolMessage = "Required";
                    }
                }

                tblUserExtended.Schools = userService.GetSelectedSchoolCheckBoxes(tblUserExtended);
                ViewBag.RoleId = new SelectList(modelService.GetRolesForRole((int)(siteUser.Role)), "RoleId", "RoleDesc", tblUserExtended.RoleId);
                FillViewBagValues(siteUser.Districts[0].Name, string.Empty, siteUser.RoleDesc, tblUserExtended.SchoolYearId);
                return View(tblUserExtended);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }

        public ActionResult Edit(int? id, int schoolYearId)
        {

            try
            {
                db = new dbTIREntities();
                modelService = new ModelServices();
                siteUser = ((SiteUser)Session["SiteUser"]);
                schoolService = new SchoolService(siteUser, db);
                userService = new UserService(siteUser, db);
                //tblUserExt tbluserExtended = null;
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblUser tbluser = db.tblUsers.Find(id);

                if (tbluser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var context = new Models.ApplicationDbContext();
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                string aspNetUserName = "--";
                if (!String.IsNullOrEmpty(tbluser.AspNetUserId))
                {
                    ApplicationUser aspNetUser = userManager.FindById(tbluser.AspNetUserId);
                    if (aspNetUser != null)
                    {
                        aspNetUserName = aspNetUser.UserName;
                    }
                }
                //Get RoleId from tblUserDistrict instead of tblUser
                int roleId = userService.GetRoleId(id, schoolYearId);
                tblUserExt tbluserExtended = new tblUserExt()
                {
                    UserId = tbluser.UserId,
                    UserName = aspNetUserName,
                    FirstName = tbluser.FirstName,
                    LastName = tbluser.LastName,
                    UserEmail = tbluser.UserEmail,
                    StateId = tbluser.StateId,
                    Schools = tbluser.Schools,
                    SchoolYearId = schoolYearId,
                    RoleId = roleId
                };

                //Check that edited user's school must be from EDSUser schools or edsUser must have permissions to view user school
                bool isUserHasPermissionForSchool = userService.IsUserHasPermissionForSchool(tbluserExtended);
                if (!isUserHasPermissionForSchool)
                {
                    return RedirectToAction("Index");
                }

                //Get User schools
                tbluserExtended.Schools = userService.GetUserSchoolWithCheckBoxes(tbluserExtended);
                var dropDownEmpty = Enumerable.Repeat(new SelectListItem { Value = "", Text = "" }, count: 1);
                FillViewBagValues(siteUser.Districts[0].Name, string.Empty, siteUser.RoleDesc, schoolYearId);
                FillUserExtendedCommanData(modelService, tbluserExtended);
                return View(tbluserExtended);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUserExt tblUserExtended)
        {
            try
            {
                db = new dbTIREntities();
                modelService = new ModelServices();
                siteUser = ((SiteUser)Session["SiteUser"]);
                userService = new UserService(siteUser, db);
                schoolService = new SchoolService(siteUser, db);
                int userAssignedDistrict = siteUser.Districts[0].Id;

                if (ModelState.IsValid)
                {
                    if (tblUserExtended.SelectedSchools != null && tblUserExtended.SelectedSchools.Count() > 0)
                    {
                        bool isEmailAddressExist = db.tblUsers.Where(x => x.UserEmail == tblUserExtended.UserEmail && x.UserId != tblUserExtended.UserId).Count() > 0 ? true : false;
                        bool isStateIdExist = db.tblUsers.Where(x => x.StateId == tblUserExtended.StateId && x.UserId != tblUserExtended.UserId).Count() > 0 ? true : false;

                        if ((!isEmailAddressExist) && (!isStateIdExist))
                        {
                            userService.UpdateUser(tblUserExtended);
                            HelperService.UpdateSiteUserProfile(siteUser, db);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            if (isEmailAddressExist)
                                ModelState.AddModelError("UserEmail", "Duplicate email - please choose a unique email.");
                            if (isStateIdExist)
                                ModelState.AddModelError("StateId", "Duplicate state id - please choose a unique state.");
                        }
                    }
                    else
                    {
                        ViewBag.SchoolMessage = "Required";
                    }
                }
                tblUserExtended.Schools = userService.GetSelectedSchoolCheckBoxes(tblUserExtended);
                FillViewBagValues(siteUser.Districts[0].Name, string.Empty, siteUser.RoleDesc, tblUserExtended.SchoolYearId);
                FillUserExtendedCommanData(modelService, tblUserExtended);
                return View(tblUserExtended);
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
                return View("GeneralError");
            }
        }


        private void FillViewBagValues(string districtName, string school, string roleDesc, int schoolYearId)
        {
            try
            {
                ViewBag.DistrictDesc = districtName;
                ViewBag.SchoolId = modelService.DropDownDataSchool(school, siteUser.EdsUserId, schoolYearId, true);
                ViewBag.AllowEdit = HelperService.AllowUiEdits(roleDesc, "USER");
            }
            catch (Exception ex)
            {
                Logging log = new Logging();
                log.LogException(ex);
            }
        }

        private void FillUserExtendedCommanData(ModelServices modelService, tblUserExt tbluserExtended)
        {
            ViewBag.RoleId = new SelectList(modelService.GetRolesForRole((int)(siteUser.Role)), "RoleId", "RoleDesc", tbluserExtended.RoleId);
            ViewBag.SchoolYear = schoolService.GetSchoolYearDownData((int)tbluserExtended.SchoolYearId);
            int currentSchoolYear = Convert.ToInt32(schoolService.GetCurrentSchoolYear());
            ViewBag.CurrentSchoolYearId = modelService.GetSchoolYearId(currentSchoolYear);
            var classData = modelService.GetClassesByTeacher((int)tbluserExtended.SchoolYearId, new[] { (int)tbluserExtended.UserId });
            tbluserExtended.SchoolClasses = new List<SchoolClass>();
            foreach (var classItem in classData)
            {
                SchoolClass classItems = new SchoolClass() { ClassDesc = classItem.Name, SchoolClassId = classItem.Id };
                tbluserExtended.SchoolClasses.Add(classItems);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

    }
}


