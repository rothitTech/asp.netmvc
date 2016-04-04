using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EDS.Controllers
{
    public class BreadcrumNavigableController : Controller
    {
        /// <summary>
        /// Include the render of this action in any cshtml where Breadcrum needs to be generated for reports.
        /// </summary>
        /// <param name="currentViewTitle">Pass the current view title to get displayed on rightmost side of the breadcrum navigation</param>
        /// <param name="addLevel">Pass the addLevel value false when need to navigates from summary report and student page</param>
        /// <returns></returns>
        public ActionResult Breadcrum(string currentViewTitle)
        {
            ViewBag.CurrentViewTitle = currentViewTitle;
            var navigations = GetBreadcrumNavigationsFromSession();
            AddDefaultRootSummaryReportNavItem(navigations);

            this.ViewBag.BreadcrumNavigations = navigations;
            return PartialView();
        }

        /// <summary>
        /// To be called from every page action which needs Breadcrum Navigation link generation.
        /// </summary>
        protected void SetNavigationLinksUrl()
        {
            var navigations = GetBreadcrumNavigationsFromSession();
            if (HandleIfRootSummaryReportPage(navigations)) return;
            HandleReferralUrlForForwardnavigationOnly(navigations);
            RemoveNavItemsIfNavigationIsDoneFromBreadcrum(navigations);
            AddDefaultRootSummaryReportNavItem(navigations);

            Session["breadcrumNavigations"] = navigations;
        }

        private void AddDefaultRootSummaryReportNavItem(List<BreadcrumItem> navigations)
        {
            if (navigations.Count == 0)
            {
                navigations.Add(new BreadcrumItem { Title = "Summary Report", NavUrl = "/TeacherSummaryReport/SummaryReport?", Level = 1 });
            }
        }
        private List<BreadcrumItem> GetBreadcrumNavigationsFromSession()
        {
            var navigations = new List<BreadcrumItem>();
            if (Session["breadcrumNavigations"] != null)
            {
                navigations = (List<BreadcrumItem>)Session["breadcrumNavigations"];
            }
            return navigations;
        }
        private bool HandleIfRootSummaryReportPage(List<BreadcrumItem> navigations)
        {
            //if summary report page, destroy navigations
            if (Request.Url.ToString().ToLower().Contains("summaryreport"))
            {
                navigations.Clear();
                Session["breadcrumNavigations"] = navigations;
                return true;
            }
            return false;
        }
        private void HandleReferralUrlForForwardnavigationOnly(List<BreadcrumItem> navigations)
        {
            var level = this.Request.QueryString["level"];
            //add ref url from where we came
            if (string.IsNullOrWhiteSpace(level) &&
                this.Request.UrlReferrer != null &&
                !string.IsNullOrWhiteSpace(this.Request.UrlReferrer.PathAndQuery))
            {
                var nav = this.Request.UrlReferrer.PathAndQuery;
                nav = HttpUtility.UrlDecode(nav);
                var currUrl = HttpUtility.UrlDecode(this.Request.Url.ToString());
               
                //Current and Previous assessmentTypeId are used to compare in case when prev and current url is same to avoid duplicate breadcrum
                //(for ex - to view next/prev assessment, termId drop down change and sorting)
                string currAssessmentTypeId = this.Request.QueryString["AssessmentTypeId"];
                var assessmentTypeParamMatch = Regex.Match(nav.ToLower(), @"assessmenttypeid=\d*");
                var assessmentTypeIdMatch = Regex.Match(assessmentTypeParamMatch.Value, @"=\d+");
                string prevAssessmentTypeId = assessmentTypeIdMatch.Value;
                prevAssessmentTypeId = Regex.Replace(prevAssessmentTypeId, @"=", "");

                var avoidAddNavigationLink = false;
                //check if page is the same as earlier page AND check if nav item was already added in list
                if (nav.Length >= 30)
                {
                    avoidAddNavigationLink = (currUrl.ToLower().Contains(nav.Substring(0, 30).ToLower()) && (currAssessmentTypeId == prevAssessmentTypeId)) ||
                       (navigations.Count > 0 && nav.ToLower().Contains(HttpUtility.UrlDecode(navigations[navigations.Count - 1].NavUrl).ToLower()));
                }
                if (!avoidAddNavigationLink)
                {
                    nav = Regex.Replace(nav, @"&level=\d", "");
                    var previousReport = Request.QueryString["cameFromTitle"];
                    //if title of navigation link is not found, then default it like "Level{0}"
                    if (string.IsNullOrWhiteSpace(previousReport)) previousReport = string.Format("Level{0}", (navigations.Count + 1));
                    navigations.Add(new BreadcrumItem { NavUrl = nav, Title = previousReport, Level = navigations.Count + 1 });
                }
            }
        }
        private void RemoveNavItemsIfNavigationIsDoneFromBreadcrum(List<BreadcrumItem> navigations)
        {
            var level = this.Request.QueryString["level"];
            if (!string.IsNullOrWhiteSpace(level))
            {
                //remove items on and after current level
                var backItem = navigations.FirstOrDefault(i => i.Level == int.Parse(level));
                if (backItem != null)
                {
                    var ind = navigations.IndexOf(backItem);
                    for (int x = navigations.Count - 1; x >= ind; x--)
                    {
                        navigations.RemoveAt(x);
                    }
                }
            }
        }
    }

    [Serializable]
    public class BreadcrumItem
    {
        public int Level { get; set; }
        public string Title { get; set; }
        public string NavUrl { get; set; }
    }
}