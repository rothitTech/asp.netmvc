using EDS.Models;
using EDS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDS.Helpers
{
    public static class ReportsFilterHelper
    {

        public static ReportFilterViewModel PopulateReportFilterViewModel(FilterParameter filterParameter, ModelServices modelService, SiteUser siteUser)
        {
            string defaultText = "All";
            var reportFilterViewModel = new ReportFilterViewModel();
            reportFilterViewModel.TeacherName = modelService.GetUserNameByUserId(filterParameter.Teacher);
            reportFilterViewModel.ClassName = filterParameter.ClassId != -1 ? modelService.GetClassDesc(filterParameter.ClassId) : defaultText;
            reportFilterViewModel.Race = (filterParameter.Race == -1) ? defaultText : modelService.GetRaceDesc(filterParameter.Race);
            reportFilterViewModel.Gender = (filterParameter.Gender == -1) ? defaultText : modelService.GetGenderDesc(filterParameter.Gender);
            //TODO: till now first record with district name and Id is picked up. Need to refactor if user belongs to multiple district. 
            reportFilterViewModel.DistrictName = siteUser.Districts.First().Name;
            reportFilterViewModel.IsAdmin = HelperService.AllowUiEdits(siteUser.RoleDesc, "REPORT");
            reportFilterViewModel.DisplayAdminFilters = DecideFilterDisplay(filterParameter);
            reportFilterViewModel.FrlIndicator = filterParameter.FrlIndicator;
            reportFilterViewModel.LepIndicator = filterParameter.LEPIndicator;
            reportFilterViewModel.IepIndicator = filterParameter.IEPIndicator;
            reportFilterViewModel.Hispanic = filterParameter.Hispanic;
            reportFilterViewModel.SchoolYear = filterParameter.SchoolYear;
            return reportFilterViewModel;
        }

        private static bool DecideFilterDisplay(FilterParameter filterParameter)
        {

            return (filterParameter.Race != -1
                || filterParameter.Gender != -1
                || filterParameter.Hispanic != null
                || filterParameter.FrlIndicator != null
                || filterParameter.IEPIndicator != null
                || filterParameter.LEPIndicator != null);
        }
    }
}