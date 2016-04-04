using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using System.Linq.Dynamic;
using EDS.Services;

using EDS.Helpers;
using System.Linq.Expressions;

namespace EDS.Models
{
    public class ModelServices : IDisposable
    {
        private readonly dbTIREntities entities = new dbTIREntities();
        private IEnumerable<SelectListItem> _dropDownDefault = Enumerable.Repeat(new SelectListItem { Value = "-1", Text = "--All--" }, count: 1);

        public IEnumerable GetTeachers()
        {
            List<tblClassTeacher> teachers = new List<tblClassTeacher>();
            return teachers.ToList();
        }

        public IEnumerable GetTeachers(int id)
        {
            List<tblClassTeacher> teachers = new List<tblClassTeacher>();
            return teachers.ToList();
        }

        public String GetTeacherSchool(string id)
        {
            return (from e in entities.SchoolTeachers
                    where e.AspNetUserId == id
                    select e.SchoolDesc).SingleOrDefault();
        }

        public int GetSchoolId(int id)
        {
            return (from e in entities.tblUserSchools
                    where e.UserId == id
                    select e.SchoolId).SingleOrDefault();
        }

        public String GetUserName(string id)
        {
            return (from e in entities.SchoolTeachers
                    where e.AspNetUserId == id
                    select e.FirstName + " " + e.LastName).SingleOrDefault();
        }

        public string GetUserNameByUserId(int id)
        {
            return (from e in entities.SchoolTeachers
                    where e.UserId == id
                    select e.FirstName + " " + e.LastName)
                    .Take(1)
                    .SingleOrDefault();
        }

        public int GetEDSUserId(string id)
        {

            return (from e in entities.tblUsers
                    where e.AspNetUserId == id
                    select e.UserId).SingleOrDefault();
        }

        public IEnumerable GetSchools()
        {
            List<tblSchool> schools = new List<tblSchool>();
            return schools.ToList();
        }

        public IEnumerable GetSchools(int id)
        {
            List<tblSchool> schools = new List<tblSchool>();
            return schools.ToList();
        }

        public List<int> GetSchoolTeacherIds(int id)
        {
            return (from e in entities.tblUserSchools
                    where e.SchoolId == id
                    select e.UserId).ToList();

        }

        public List<int> GetClassIds(int id)
        {
            return (from e in entities.tblClasses
                    where e.SchoolId == id
                    select e.ClassId).ToList();
        }


        public string GetDistrictName(int id)
        {

            return (from e in entities.tblDistricts
                    where e.DistrictId == id
                    select e.DistrictDesc).SingleOrDefault();
        }

        public bool UserIsAdmin(int id)
        {
            var Userrole = (from e in entities.tblUsers
                            where e.UserId == id
                            select e.RoleId).SingleOrDefault();

            if (Userrole == 2)
            {
                return true;
            }

            return false;

        }

        public void Dispose()
        {
            entities.Dispose();
        }

        /// <summary>
        /// GetDetailReport is used to get detail report data.
        /// </summary>
        /// <param name="assessmentTypeId"></param>
        /// <param name="assessmentList"></param>
        /// <param name="districtId"></param>
        /// <param name="detailReportParameter"></param>
        /// <returns></returns>
        internal List<TIRDetail> GetDetailReport(List<string> assessmentList, int districtId, TIRDetailReportParameter detailReportParameter)
        {
            List<TIRDetail> results = new List<TIRDetail>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetTeacherImpactDetail", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@SubjectId", detailReportParameter.Subject));
            command.Parameters.Add(new SqlParameter("@SchoolYearId", detailReportParameter.Year));
            command.Parameters.Add(new SqlParameter("@AssessmentTypeId", detailReportParameter.AssessmentTypeId));
            command.Parameters.Add(new SqlParameter("@TeacherId", detailReportParameter.Teacher));
            command.Parameters.Add(new SqlParameter("@GradeLevel", detailReportParameter.Grade));
            command.Parameters.Add(new SqlParameter("@ViewScaledScore", detailReportParameter.ViewScaledScore));
            command.Parameters.Add(new SqlParameter("@ClassID", detailReportParameter.ClassId));
            command.Parameters.Add(new SqlParameter("@DistrictId", districtId));
            command.Parameters.Add(new SqlParameter("@Race", detailReportParameter.Race == -1 ? (int?)null : detailReportParameter.Race));
            command.Parameters.Add(new SqlParameter("@Gender", detailReportParameter.Gender == -1 ? (int?)null : detailReportParameter.Gender));
            command.Parameters.Add(new SqlParameter("@FrlIndicator", detailReportParameter.FrlIndicator));
            command.Parameters.Add(new SqlParameter("@IepIndicator", detailReportParameter.IEPIndicator));
            command.Parameters.Add(new SqlParameter("@LepIndicator", detailReportParameter.LEPIndicator));
            command.Parameters.Add(new SqlParameter("@Hispanic", detailReportParameter.Hispanic));

            SqlDataReader reader = command.ExecuteReader();

            TIRDetail tempDetail = null;
            int prevStudent = -1;
            int assessmentIndex = 0;


            while (reader.Read())
            {
                var currentStudentId = Convert.ToInt32(reader["StudentId"]);
                if (prevStudent != currentStudentId)
                {
                    prevStudent = currentStudentId;

                    if (tempDetail != null)
                    {
                        results.Add(tempDetail);
                    }

                    tempDetail = new TIRDetail();
                    tempDetail.StudentId = currentStudentId;
                    tempDetail.LocalId = reader["LocalId"].ToString();
                    tempDetail.StudentName = reader["StudentName"].ToString();
                    tempDetail.LastName = reader["LastName"].ToString();
                    tempDetail.RubricFileName = reader["RubricFileName"].ToString();
                    tempDetail.SLOFileName = reader["SLOFileName"].ToString();
                }
                assessmentIndex = assessmentList.IndexOf(reader["AssessmentDesc"].ToString());

                decimal? score = null;
                decimal? projection = null;
                decimal? impact = null;
                decimal? percentile = null;
                int? meetExceedValue = null;
                decimal? growth = null;
                SetAssessmentScoreData(out score, out projection, out impact, out percentile, out meetExceedValue, out growth, detailReportParameter.ViewScaledScore, reader);
                try
                {
                    var propertyIndex = assessmentIndex + 1;
                    var scoreData = tempDetail.ScoreData1;
                    if (propertyIndex == 2) scoreData = tempDetail.ScoreData2;
                    if (propertyIndex == 3) scoreData = tempDetail.ScoreData3;
                    if (propertyIndex == 4) scoreData = tempDetail.ScoreData4;
                    if (propertyIndex == 5) scoreData = tempDetail.ScoreData5;

                    scoreData.RoundingType = HelperService.GetDecimalDisplayFormat(detailReportParameter.ViewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.NoDecimalPlace);
                    scoreData.Growth = growth;
                    scoreData.Percentile = percentile;
                    scoreData.Score = score;
                    scoreData.Projection = projection;
                    scoreData.Impact = impact;
                    scoreData.MeetExceedValue = meetExceedValue;
                }
                catch (Exception ex)
                {
                    throw new Exception("Check if field exists belonging to correct assessment", ex);
                }
            }

            results.Add(tempDetail);

            reader.Close();
            connection.Close();
            return results;
        }


        private void SetAssessmentScoreData(out decimal? score, out decimal? projection, out decimal? impact, out decimal? percentile, out int? meetExceedValue, out decimal? growth, bool viewScaledScore, SqlDataReader reader)
        {
            score = (reader["Score"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Score"]));
            projection = (reader["Projection"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Projection"]));
            impact = (reader["Impact"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Impact"]));
            percentile = (reader["DistrictPercentile"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["DistrictPercentile"]));
            meetExceedValue = (reader["MeetExceedValue"] == DBNull.Value) ? (int?)null : Convert.ToInt32(reader["MeetExceedValue"]);
            growth = (reader["GrowthCalc"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["GrowthCalc"]));
        }

        internal List<TIRSummary> GetSummaryReport(FilterParameter filterParameter)
        {
            List<TIRSummary> tirSummaryList = new List<TIRSummary>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetTeacherSummaryView", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@teacherId", filterParameter.Teacher));
            command.Parameters.Add(new SqlParameter("@schoolId", filterParameter.School));
            command.Parameters.Add(new SqlParameter("@schoolYearId", filterParameter.Year));
            command.Parameters.Add(new SqlParameter("@classId", filterParameter.ClassId));
            command.Parameters.Add(new SqlParameter("@ViewMeetExceedSummary", filterParameter.ViewMeetExceedSummary));
            command.Parameters.Add(new SqlParameter("@Race", filterParameter.Race == -1 ? (int?)null : filterParameter.Race));
            command.Parameters.Add(new SqlParameter("@Gender", filterParameter.Gender == -1 ? (int?)null : filterParameter.Gender));
            command.Parameters.Add(new SqlParameter("@FrlIndicator", filterParameter.FrlIndicator));
            command.Parameters.Add(new SqlParameter("@IepIndicator", filterParameter.IEPIndicator));
            command.Parameters.Add(new SqlParameter("@LepIndicator", filterParameter.LEPIndicator));
            command.Parameters.Add(new SqlParameter("@Hispanic", filterParameter.Hispanic));
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                TIRSummary tempSummary = new TIRSummary();
                tempSummary.GradeLevel = int.Parse(reader["Grade"].ToString());
                tempSummary.TeacherId = filterParameter.Teacher;
                tempSummary.YearId = filterParameter.Year;
                tempSummary.ClassId = filterParameter.ClassId;
                tempSummary.SubjectId = int.Parse(reader["SubjectId"].ToString());
                tempSummary.AssessmentCode = reader["AssessmentCode"].ToString();
                tempSummary.SubjectDesc = reader["ClassSubject"].ToString();
                tempSummary.Weighting = HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["Weighting"]));
                tempSummary.AssessmentTypeId = reader["AssessmentTypeId"].ToString();
                tempSummary.Impact = reader["Impact"] == DBNull.Value ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["Impact"]));
                tempSummary.NoOfStudent = reader["NoOfStudent"] == DBNull.Value ? 0 : int.Parse(reader["NoOfStudent"].ToString());
                tempSummary.Average = reader["Average"] == DBNull.Value ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["Average"]));
                tempSummary.AssessmentGradeWeightingId = int.Parse(reader["AssessmentGradeWeightingId"].ToString());
                tempSummary.MeetExceedPerc = reader["MeetExceedPerc"] == DBNull.Value ? (decimal)0.0 : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["MeetExceedPerc"]));
                tempSummary.AverageOrMEValue = filterParameter.ViewMeetExceedSummary ? tempSummary.MeetExceedPerc : tempSummary.Average;
                tempSummary.MeetExceedPoints = reader["MeetExceedPoints"] == DBNull.Value ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.DoubleDecimalplace, Convert.ToDecimal(reader["MeetExceedPoints"]));
                tempSummary.MeetExceedCategory = reader["MeetExceedCategory"] == DBNull.Value ? (int?)null : int.Parse(reader["MeetExceedCategory"].ToString());
                tempSummary.WeightedImpact = filterParameter.ViewMeetExceedSummary ? CalculateWeightedImpact(tempSummary.MeetExceedPoints, tempSummary.Weighting) : CalculateWeightedImpact(tempSummary.Average, tempSummary.Weighting);
                tempSummary.IsAssessmentExist = (bool)reader["IsAssessmentExist"];
                tempSummary.ReportTemplateId = Convert.ToInt32(reader["ReportTemplateId"]);
                tempSummary.AssessmentTypeDesc = Convert.ToString(reader["AssessmentTypeDesc"]);
                tirSummaryList.Add(tempSummary);
            }

            reader.Close();
            connection.Close();

            return tirSummaryList;
        }

        internal List<Weighting> GetWeightingSummary(int districtId, int yearId)
        {
            List<Weighting> WeighSummaryList = new List<Weighting>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetWeightingSummary", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@districtId", districtId));
            command.Parameters.Add(new SqlParameter("@schoolYearId", yearId));

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Weighting tempSummary = new Weighting();
                tempSummary.GradeLevel = int.Parse(reader["Grade"].ToString());
                tempSummary.YearId = yearId;
                tempSummary.SubjectId = int.Parse(reader["SubjectId"].ToString());
                tempSummary.AssessmentCode = reader["AssessmentCode"].ToString();
                tempSummary.SubjectDesc = reader["ClassSubject"].ToString();
                tempSummary.Weight = decimal.Parse(reader["Weighting"].ToString());
                tempSummary.AssessmentType = reader["AssessmentTypeId"].ToString();

                WeighSummaryList.Add(tempSummary);
            }

            reader.Close();
            connection.Close();

            return WeighSummaryList;
        }

        public List<int> GetDistrictsByUserId(int userId)
        {
            return (from userDistrict in entities.tblUserDistricts
                    join district in entities.tblDistricts on userDistrict.DistrictId equals district.DistrictId
                    where userDistrict.UserId == userId
                    select district.DistrictId).ToList();
        }

        public List<DropDownIdName> SchoolYearDropDownData()
        {
            List<DropDownIdName> results = (from yearItem in entities.tblSchoolYears
                                            orderby yearItem.SchoolYear descending
                                            select new DropDownIdName()
                                            {
                                                Id = yearItem.SchoolYearId,
                                                Name = yearItem.SchoolYearDesc
                                            }).ToList();
            return results;
        }

        public List<DropDownIdName> DistrictDropDownDataByUser(int userId)
        {
            List<DropDownIdName> results = (from userDistrict in entities.tblUserDistricts
                                            join district in entities.tblDistricts on userDistrict.DistrictId equals district.DistrictId
                                            where userDistrict.UserId == userId
                                            select new DropDownIdName()
                                            {
                                                Id = district.DistrictId,
                                                Name = district.DistrictDesc
                                            }).ToList();
            return results;
        }

        public List<DropDownIdName> DistrictDropDownData()
        {
            List<DropDownIdName> results = (from district in entities.tblDistricts
                                            select new DropDownIdName()
                                            {
                                                Id = district.DistrictId,
                                                Name = district.DistrictDesc
                                            }).ToList();
            return results;
        }

        public List<DropDownIdName> DropDownAllOnly()
        {
            return new List<DropDownIdName>() { 
                new DropDownIdName(){
                    Id = -1,
                    Name = "--All--"
                }
            };
        }

        public List<DropDownIdName> SchoolDropDownDataByDistrict(int[] districtId)
        {
            List<DropDownIdName> results = (
                from school in entities.tblSchools
                where districtId.Contains(school.DistrictId)
                select new DropDownIdName()
                {
                    Id = school.SchoolId,
                    Name = school.SchoolDesc
                }).ToList();
            return results;
        }

        public List<DropDownIdName> SchoolDropDownDataByUser(int UserId)
        {
            List<DropDownIdName> results = (
                from school in entities.tblSchools
                join schoolUser in entities.tblUserSchools on school.SchoolId equals schoolUser.SchoolId
                where schoolUser.UserId == UserId
                select new DropDownIdName()
                {
                    Id = school.SchoolId,
                    Name = school.SchoolDesc
                }).OrderBy(x => x.Name).ToList();
            return results;
        }

        public IEnumerable DropDownDataSchool(string selectedItem)
        {
            SiteUser su = ((SiteUser)HttpContext.Current.Session["SiteUser"]);
            IQueryable schoolQuery;
            var districtsForUser = su.Districts.Select(x => x.Id).ToList();
            List<int> schoolsForUser = su.Schools.Select(x => x.Id).ToList();

            if (su.isTeacher || su.isAdministrator)
            {
                schoolQuery = entities.tblSchools
                    .Where(x => districtsForUser.Contains(x.DistrictId))
                    .Where(x => schoolsForUser.Contains(x.SchoolId));
            }
            else
            {
                schoolQuery = entities.tblSchools
                    .Where(x => districtsForUser.Contains(x.DistrictId));
            }

            return _dropDownDefault.Concat(new MultiSelectList(
                    schoolQuery,
                    "SchoolId", "SchoolDesc", new string[] { selectedItem }));
        }

        public int[] GetUserDistrictsFilter(int userId, int districtId)
        {
            if (districtId == -1)
            {
                return GetDistrictsByUserId(userId).ToArray();
            }
            else
            {
                return new int[] { districtId };
            }
        }

        public int[] GetUserSchoolsFilter(int userId, int schoolId)
        {
            if (schoolId == -1)
            {
                // get user's districts
                int[] userDistricts = (from userDistrict in entities.tblUserDistricts
                                       join district in entities.tblDistricts on userDistrict.DistrictId equals district.DistrictId
                                       where userDistrict.UserId == userId
                                       select district.DistrictId).ToArray();

                return (from school in entities.tblSchools
                        where userDistricts.Contains(school.DistrictId)
                        select school.SchoolId).ToArray();
            }
            else
            {
                return new int[] { schoolId };
            }
        }

        internal List<StudentHistory> GetStudentHistoryReport(int studentId, bool showScaledScale, int districtId)
        {
            List<StudentHistory> result = new List<StudentHistory>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetStudentAssessmentHistory", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@StudentId", studentId));
            command.Parameters.Add(new SqlParameter("@ViewScaledScore", showScaledScale));
            command.Parameters.Add(new SqlParameter("@DistrictId", districtId));


            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                StudentHistory tempHistory = new StudentHistory();
                tempHistory.Subject = reader["SubjectDesc"].ToString();
                tempHistory.AssessmentCode = reader["AssessmentCode"].ToString();
                tempHistory.AssessmentDesc = reader["AssessmentDesc"].ToString();
                tempHistory.Score = (reader["Score"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(showScaledScale ? RoundingType.SiteDefaultDecimalPlace : RoundingType.NoDecimalPlace, Convert.ToDecimal(reader["Score"]));
                tempHistory.NationalPct = reader["NationalPercentile"] == DBNull.Value ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["NationalPercentile"]));
                tempHistory.DistrictPct = reader["DistrictPercentile"] == DBNull.Value ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["DistrictPercentile"]));
                tempHistory.Projection = (reader["Projection"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(showScaledScale ? RoundingType.SiteDefaultDecimalPlace : RoundingType.NoDecimalPlace, Convert.ToDecimal(reader["Projection"]));
                tempHistory.Impact = (reader["Impact"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(showScaledScale ? RoundingType.SiteDefaultDecimalPlace : RoundingType.NoDecimalPlace, Convert.ToDecimal(reader["Impact"]));
                tempHistory.Growth = (reader["Growth"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["Growth"]));
                tempHistory.MeetExceedValue = (reader["MeetExceedValue"] == DBNull.Value) ? (int?)null : Convert.ToInt32(reader["MeetExceedValue"]);
                result.Add(tempHistory);
            }

            reader.Close();
            connection.Close();
            return result;
        }

        public string SchoolYearDescription()
        {
            int currentSchoolYear = Convert.ToInt32(ConfigurationManager.AppSettings["SchoolYear"].ToString());
            var schoolYear = entities.tblSchoolYears.Where(y => y.SchoolYear == currentSchoolYear)
                .Select(d => d.SchoolYearDesc).ToList();
            return schoolYear[0].ToString();
        }

        public string SchoolYearDescriptionByYearId(int yearId)
        {
            var schoolYear = entities.tblSchoolYears.Where(y => y.SchoolYearId == yearId)
                .Select(d => d.SchoolYearDesc).ToList();
            return schoolYear[0].ToString();
        }

        public int SchoolYearId()
        {
            int currentSchoolYear = Convert.ToInt32(ConfigurationManager.AppSettings["SchoolYear"].ToString());
            int schoolYearId = entities.tblSchoolYears.Where(y => y.SchoolYear == currentSchoolYear)
                             .Select(d => d.SchoolYearId).FirstOrDefault();
            return schoolYearId;
        }

        public int GetSchoolYearByYearId(int schoolYearId)
        {
            var schoolYear = entities.tblSchoolYears.Where(y => y.SchoolYearId == schoolYearId)
                .Select(d => d.SchoolYear).FirstOrDefault();
            return schoolYear;
        }

        public IEnumerable GetUserSchoolYear(int userId, int districtId, int selectedItem)
        {
            var query = from userDistrict in entities.tblUserDistricts
                        join schoolYear in entities.tblSchoolYears on userDistrict.SchoolYearId equals schoolYear.SchoolYearId
                        where userDistrict.UserId == userId && userDistrict.DistrictId == districtId
                        orderby schoolYear.SchoolYear descending
                        select schoolYear;
            return new MultiSelectList(query, "SchoolYearId", "SchoolYearDesc", new int[] { selectedItem });

        }

        /// <summary>
        /// GetDetailReportAssessmentList
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="assessmentTypeId"></param>
        /// <param name="detailReportParameter"></param>
        /// <returns></returns>
        internal List<AssessmentMEPerc> GetDetailReportAssessmentList(int districtId, TIRDetailReportParameter detailReportParameter)
        {
            List<AssessmentMEPerc> results = new List<AssessmentMEPerc>();

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetTeacherAssessmentWithMEPerc", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@DistrictId", districtId));
            command.Parameters.Add(new SqlParameter("@SubjectId", detailReportParameter.Subject));
            command.Parameters.Add(new SqlParameter("@SchoolYearId", detailReportParameter.Year));
            command.Parameters.Add(new SqlParameter("@AssessmentTypeId", detailReportParameter.AssessmentTypeId));
            command.Parameters.Add(new SqlParameter("@TeacherId", detailReportParameter.Teacher));
            command.Parameters.Add(new SqlParameter("@GradeLevel", detailReportParameter.Grade));
            command.Parameters.Add(new SqlParameter("@ClassID", detailReportParameter.ClassId));
            command.Parameters.Add(new SqlParameter("@Race", detailReportParameter.Race == -1 ? (int?)null : detailReportParameter.Race));
            command.Parameters.Add(new SqlParameter("@Gender", detailReportParameter.Gender == -1 ? (int?)null : detailReportParameter.Gender));
            command.Parameters.Add(new SqlParameter("@FrlIndicator", detailReportParameter.FrlIndicator));
            command.Parameters.Add(new SqlParameter("@IepIndicator", detailReportParameter.IEPIndicator));
            command.Parameters.Add(new SqlParameter("@LepIndicator", detailReportParameter.LEPIndicator));
            command.Parameters.Add(new SqlParameter("@Hispanic", detailReportParameter.Hispanic));

            SqlDataReader reader = command.ExecuteReader();

            AssessmentMEPerc tempAssessmentME = null;

            while (reader.Read())
            {
                tempAssessmentME = new AssessmentMEPerc();
                tempAssessmentME.AssessmentDesc = reader["AssessmentDesc"].ToString();
                tempAssessmentME.MeetExceedPerc = reader["MeetExceedPerc"] == DBNull.Value ? (decimal)0.0 : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["MeetExceedPerc"]));
                tempAssessmentME.MeetExceedCategory = reader["MeetExceedCategory"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["MeetExceedCategory"]);
                tempAssessmentME.SchoolTermId = Convert.ToInt32(reader["TermId"]);
                tempAssessmentME.AssessmentId = Convert.ToInt32(reader["AssessmentId"]);
                tempAssessmentME.AssessmentTypeId = detailReportParameter.AssessmentTypeId;
                results.Add(tempAssessmentME);
            }

            reader.Close();
            connection.Close();
            return results;
        }

        internal string GetEmailByUserId(string userName)
        {
            string email = (from tempUser in entities.tblUsers
                            join tempAspUser in entities.AspNetUsers on tempUser.AspNetUserId equals tempAspUser.Id
                            where tempAspUser.UserName == userName
                            select tempUser.UserEmail).FirstOrDefault();

            return email;
        }

        internal string GeneratePasswordResetToken(string user, string email)
        {
            string newToken = Guid.NewGuid().ToString();
            entities.tblUserPasswordResetTokens.Add(new tblUserPasswordResetToken() { ResetEmail = email, ResetToken = newToken, ResetUser = user, CreatedDateTime = DateTime.Now });
            entities.SaveChanges();

            return newToken;
        }

        internal void EmailPasswordReset(string email, string token, string id)
        {
            string baseURL = ConfigurationManager.AppSettings["systemBaseURL"].ToString();
            StringBuilder builder = new StringBuilder();
            builder.Append("Please click on the link below to reset your password.<br/> <a href='");
            builder.Append(baseURL);
            builder.Append(@"/account/ResetPasswordConfirmation?token=");
            builder.Append(token);
            builder.Append(@"&id=");
            builder.Append(id);
            builder.Append(@"'>Reset Password</a>");
            SmtpClient mailClient = new SmtpClient(ConfigurationManager.AppSettings["smtpServer"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"].ToString()));
            MailMessage message = new MailMessage(ConfigurationManager.AppSettings["returnEmail"].ToString(), email);
            message.IsBodyHtml = true;
            message.Body = builder.ToString();
            message.Subject = "Password Reset Confirmation";
            mailClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["AWSsmtpUser"].ToString(), ConfigurationManager.AppSettings["AWSsmtpPwd"].ToString());
            mailClient.EnableSsl = true;
            mailClient.Send(message);
        }


        internal List<DropDownIdName> TeacherDropDownDataBySchoolAndYear(int[] schoolId, int schoolYearId, int districtId)
        {

            IQueryable<DropDownIdName> query = (
                        from u in entities.tblUsers
                        join us in entities.tblUserSchools on u.UserId equals us.UserId
                        join ud in entities.tblUserDistricts on u.UserId equals ud.UserId
                        where schoolId.Contains(us.SchoolId) && us.SchoolYearId == schoolYearId && ud.DistrictId == districtId
                        select new DropDownIdName()
                        {
                            Id = u.UserId,
                            Name = u.FirstName + " " + u.LastName,
                            FirstName = u.FirstName,
                            LastName = u.LastName
                        });

            return query.Distinct().OrderBy(q => q.LastName.ToUpper()).ThenBy(q => q.FirstName.ToUpper()).ToList();
        }

        internal List<int> getSchoolsByUserId(int userId)
        {
            return (from teacherschool in entities.tblUserSchools
                    where teacherschool.UserId == userId
                    select teacherschool.SchoolId).ToList();
        }

        internal List<int> getTeachersBySchoolsId(int[] schoolId)
        {
            return (from teacher in entities.tblUsers
                    join teacherschool in entities.tblUserSchools
                    on teacher.UserId equals teacherschool.UserId
                    where schoolId.Contains(teacherschool.SchoolId)
                    select teacher.UserId).ToList();
        }

        internal bool IsResetTokenValid(string token, string id)
        {
            bool result = false;

            tblUserPasswordResetToken entry = (from tokenItem in entities.tblUserPasswordResetTokens
                                               where tokenItem.ResetToken == token
                                               select tokenItem).FirstOrDefault();

            if (entry != null && entry.CreatedDateTime > DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)))
            {
                result = true;
            }
            return result;
        }

        internal string GetAspUserIdForToken(string token)
        {
            return (from tokenItem in entities.tblUserPasswordResetTokens
                    join aspRec in entities.AspNetUsers on tokenItem.ResetUser equals aspRec.UserName
                    where tokenItem.ResetToken == token
                    select aspRec.Id).FirstOrDefault();
        }

        internal string GetUserIdByUserName(string userNm)
        {
            return (from user in entities.AspNetUsers
                    where user.UserName == userNm
                    select user.Id).FirstOrDefault();
        }

        internal tblStudent GetStudentById(int studentId)
        {
            return (from student in entities.tblStudents
                    where student.StudentId == studentId
                    select student).FirstOrDefault();
        }

        internal string GetSchoolNameByStudentId(int studentId)
        {
            var query = (from school in entities.tblSchools
                         join studentSchoolYear in entities.tblStudentSchoolYears on school.SchoolId equals studentSchoolYear.ServingSchoolId
                         join schoolYear in entities.tblSchoolYears on studentSchoolYear.SchoolYearId equals schoolYear.SchoolYearId
                         where studentSchoolYear.StudentId == studentId
                         select new { school.SchoolDesc, schoolYear.SchoolYear }).OrderByDescending(x => x.SchoolYear);

            return query.FirstOrDefault().SchoolDesc;
        }

        /// <summary>
        /// Returns valid roles that a particular role can assign to users
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable GetRolesForRole(int roleId)
        {
            int edsRoleId = int.Parse(ConfigurationManager.AppSettings["EdsAdminRoleId"].ToString());

            if (roleId != edsRoleId)
            {
                return from role in entities.tblRoles
                       where role.RoleId != edsRoleId
                       select role;
            }
            else
            {
                return from role in entities.tblRoles select role;
            }
        }


        internal string GetSubjectDescriptionById(int subject)
        {
            return (from tempSubject in entities.tblSubjects
                    where tempSubject.SubjectId == subject
                    select tempSubject.SubjectDesc).FirstOrDefault();
        }

        public List<DropDownIdName> GetClassesByTeacher(int yearId, int[] teachersId)
        {
            var query = (from schoolClass in entities.tblClasses
                         join teacher in entities.tblClassTeachers on schoolClass.ClassId equals teacher.ClassId
                         where schoolClass.SchoolYearId == yearId
                         && teachersId.Contains(teacher.UserId)
                         select schoolClass).AsEnumerable();

            return query.Select(x => new DropDownIdName()
                                {
                                    Id = x.ClassId,
                                    Name = x.ClassDesc
                                }
                               ).Distinct(new DropDownCompare()).OrderBy(q => q.Name.ToUpper()).ToList();
        }

        public List<SchoolClass> GetClassesByStudent(int studentId, int schoolYear)
        {
            var query = (from classes in entities.tblClasses
                         join studentclass in entities.tblClassStudents on classes.ClassId equals studentclass.ClassId
                         join schoolYears in entities.tblSchoolYears on classes.SchoolYearId equals schoolYears.SchoolYearId
                         where studentclass.StudentId == studentId && schoolYears.SchoolYear == schoolYear
                         select classes).AsEnumerable();

            return query.Select(x => new SchoolClass()
                                {
                                    SchoolClassId = x.ClassId,
                                    ClassDesc = x.ClassDesc
                                }).OrderBy(q => q.ClassDesc.ToUpper()).ToList();
        }

        public List<SchoolYear> GetSchoolYearByStudent(int studentId)
        {
            var query = (from studentSchoolYear in entities.tblStudentSchoolYears
                         join schoolYear in entities.tblSchoolYears on studentSchoolYear.SchoolYearId equals schoolYear.SchoolYearId
                         where studentSchoolYear.StudentId == studentId
                         select schoolYear).AsEnumerable();

            return query.Select(x => new SchoolYear()
                                {
                                    SchoolYearId = x.SchoolYearId,
                                    SchoolYearCode = x.SchoolYear,
                                    SchoolYearDesc = x.SchoolYearDesc
                                }).ToList();
        }

        internal string GetClassDesc(int classId)
        {
            return entities.tblClasses.FirstOrDefault(x => x.ClassId == classId).ClassDesc;
        }
        internal string GetRaceDesc(int raceId)
        {
            return entities.tblRaces.FirstOrDefault(x => x.RaceId == raceId).RaceCode;
        }
        internal string GetGenderDesc(int genderId)
        {
            return entities.tblGenders.FirstOrDefault(x => x.GenderId == genderId).GenderDesc;
        }

        public IEnumerable GetGradeCategoryByGradeWaitingID(int GradeWaitingID)
        {
            var query = (from GradeWeightingCategory in entities.tblAssessmentGradeWeightingCategories
                         join WeightingCategory in entities.tblWeightingCategories on GradeWeightingCategory.CategoryId equals WeightingCategory.CategoryId
                         where GradeWeightingCategory.AssessmentGradeWeightingId == GradeWaitingID
                         orderby WeightingCategory.CategoryId
                         select new { Min = GradeWeightingCategory.Min, Max = GradeWeightingCategory.Max, Category = WeightingCategory.CategoryDesc }
                         ).AsEnumerable();

            return query;
        }

        internal decimal CalculateWeightedImpact(decimal? Impact, decimal? Weight)
        {
            decimal WeightedImpact;

            if (Impact != null)
            {
                WeightedImpact = HelperService.GetRoundedValue(RoundingType.DoubleDecimalplace, Convert.ToDecimal((Impact * Weight) / 100));
            }
            else
            {
                WeightedImpact = HelperService.GetRoundedValue(RoundingType.DoubleDecimalplace, Convert.ToDecimal(0.00));
            }

            return WeightedImpact;

        }

        public int? GetMeetExceedImpactCategory(decimal? TotalImpactScore)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("select dbo.udfGetMeetExceedImpactCategory(" + TotalImpactScore + ") as MeetExceedCategory", connection);
            command.CommandType = System.Data.CommandType.Text;
            var scalarValue = command.ExecuteScalar();
            connection.Close();
            int? meetExceedCategory = scalarValue == DBNull.Value ? null : (int?)Convert.ToInt32(scalarValue);
            return meetExceedCategory;
        }

        /// <summary>
        /// Pulls all rows from tblGender and wraps results
        /// as MultiSelectList. 
        /// </summary>
        public List<DropDownIdDesc> DropDownDataForGender()
        {
            IQueryable<DropDownIdDesc> query = (from gender in entities.tblGenders
                                                select new DropDownIdDesc()
                                                {
                                                    Id = gender.GenderId,
                                                    Desc = gender.GenderDesc
                                                });
            return query.OrderBy(q => q.Desc).ToList();
        }


        /// <summary>
        /// Pulls all rows from tblRace and wraps results
        /// as MultiSelectList. 
        /// </summary>
        public List<DropDownIdDesc> DropDownDataForRace()
        {
            IQueryable<DropDownIdDesc> query = (from race in entities.tblRaces
                                                select new DropDownIdDesc()
                                                {
                                                    Id = race.RaceId,
                                                    Desc = race.RaceCode
                                                });
            return query.OrderBy(q => q.Desc).ToList();
        }

        /// <summary>
        /// GetReportTemplateConfigurations function fetch configurable data for a template. 
        /// </summary>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetReportTemplateConfigurations(int templateId)
        {

            string strKey = "ReportTemplateConfigurableValues";
            Dictionary<string, string> templateConfigurationData = null;
            var cachedData = CacheHelper.GetFromCache(strKey);
            if (cachedData != null)
            {
                var data = (List<tblReportTemplateConfiguration>)cachedData;
                templateConfigurationData = GetConfigData(data, templateId);
            }
            else
            {
                if (templateConfigurationData == null)
                {
                    string cacheExpiry = ConfigurationManager.AppSettings["ReportTemplateConfigsCacheIntervalMinutes"];
                    var cacheData = entities.tblReportTemplateConfigurations.ToList();
                    CacheHelper.AddToCache(strKey, cacheExpiry, cacheData);
                    templateConfigurationData = GetConfigData(cacheData, templateId);
                }
            }

            return templateConfigurationData; ;
        }

        private Dictionary<string, string> GetConfigData(List<tblReportTemplateConfiguration> templateConfigurations, int templateId)
        {
            return (templateConfigurations.Where(x => x.TemplateId == templateId)
                                            .Select(t => new { t.Key, t.Value })
                                            .ToDictionary(t => t.Key, t => t.Value));
        }

        public List<DropDownIdName> GetSchoolDropDownData(int userId, int schoolYearId)
        {
            List<DropDownIdName> results = (
                from school in entities.tblSchools
                join schoolUser in entities.tblUserSchools on school.SchoolId equals schoolUser.SchoolId
                where schoolUser.UserId == userId && schoolUser.SchoolYearId == schoolYearId
                select new DropDownIdName()
                {
                    Id = school.SchoolId,
                    Name = school.SchoolDesc
                }).OrderBy(x => x.Name).ToList();
            return results;
        }

        public int GetSchoolYearId(int schoolYear)
        {
            return entities.tblSchoolYears.Where(y => y.SchoolYear == schoolYear).Select(x => x.SchoolYearId).FirstOrDefault();
        }


        /// <summary>
        /// GetDetailReport is used to get detail report data.
        /// </summary>
        /// <param name="assessmentTypeId"></param>
        /// <param name="assessmentList"></param>
        /// <param name="districtId"></param>
        /// <param name="detailReportParameter"></param>
        /// <returns></returns>
        internal List<HeirarchicalTIRDetail> GetHeirarchicalDetailReport(int districtId, TIRDetailReportParameter detailReportParameter)
        {
            //int horizontalPageIndex = 0;

            List<HeirarchicalTIRDetail> results = new List<HeirarchicalTIRDetail>();
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetHeirarchicalTeacherImpactDetail", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@SubjectId", detailReportParameter.Subject));
            command.Parameters.Add(new SqlParameter("@SchoolYearId", detailReportParameter.Year));
            command.Parameters.Add(new SqlParameter("@AssessmentTypeId", detailReportParameter.AssessmentTypeId));
            command.Parameters.Add(new SqlParameter("@TeacherId", detailReportParameter.Teacher));
            command.Parameters.Add(new SqlParameter("@GradeLevel", detailReportParameter.Grade));
            command.Parameters.Add(new SqlParameter("@ViewScaledScore", detailReportParameter.ViewScaledScore));
            command.Parameters.Add(new SqlParameter("@ClassID", detailReportParameter.ClassId));
            command.Parameters.Add(new SqlParameter("@DistrictId", districtId));
            command.Parameters.Add(new SqlParameter("@InputTermId", detailReportParameter.InputTermId));
            command.Parameters.Add(new SqlParameter("@Race", detailReportParameter.Race == -1 ? (int?)null : detailReportParameter.Race));
            command.Parameters.Add(new SqlParameter("@Gender", detailReportParameter.Gender == -1 ? (int?)null : detailReportParameter.Gender));
            command.Parameters.Add(new SqlParameter("@FrlIndicator", detailReportParameter.FrlIndicator));
            command.Parameters.Add(new SqlParameter("@IepIndicator", detailReportParameter.IEPIndicator));
            command.Parameters.Add(new SqlParameter("@LepIndicator", detailReportParameter.LEPIndicator));
            command.Parameters.Add(new SqlParameter("@Hispanic", detailReportParameter.Hispanic));
            command.Parameters.Add(new SqlParameter("@InputParentAssessmentTypeId", detailReportParameter.InputParentAssessmentTypeId));

            SqlDataReader reader = command.ExecuteReader();

            HeirarchicalTIRDetail tempDetail = null;
            int prevStudent = -1;
            int assessmentIndex = 0;
            while (reader.Read())
            {
                var currentStudentId = Convert.ToInt32(reader["StudentId"]);
                if (prevStudent != currentStudentId)
                {
                    prevStudent = currentStudentId;

                    if (tempDetail != null)
                    {
                        //creating new row
                        tempDetail.AssessmentCount = assessmentIndex;
                        results.Add(tempDetail);
                        assessmentIndex = 0;
                    }

                    tempDetail = new HeirarchicalTIRDetail();
                    tempDetail.StudentId = currentStudentId;
                    tempDetail.LocalId = reader["LocalId"].ToString();
                    tempDetail.StudentName = reader["StudentName"].ToString();
                    tempDetail.LastName = reader["LastName"].ToString();
                    tempDetail.SchoolTermId = reader["SchoolTermId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["SchoolTermId"]);
                    tempDetail.RubricFileName = reader["RubricFileName"].ToString();
                    tempDetail.SLOFileName = reader["SLOFileName"].ToString();
                }

                decimal? score = null;
                decimal? projection = null;
                decimal? impact = null;
                decimal? percentile = null;
                int? meetExceedValue = null;
                decimal? growth = null;
                int? parentAssessmentTypeId = null;
                int assessmentId;
                int assessmentTypeId;
                string assessmentDesc = null;
                string assessmentCode = null;
                SetHeirarchicalAssessmentScoreData(out score, out projection, out impact, out percentile, out meetExceedValue, out growth, out parentAssessmentTypeId, out assessmentId, out assessmentTypeId, out assessmentDesc, out assessmentCode, detailReportParameter.ViewScaledScore, reader);
                try
                {
                    var scoreData = new HeirarchicalScoreData();
                    tempDetail.ScoreDataCollection.Add(scoreData);

                    scoreData.RoundingType = HelperService.GetDecimalDisplayFormat(detailReportParameter.ViewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.NoDecimalPlace);
                    scoreData.Growth = growth;
                    scoreData.Percentile = percentile;
                    scoreData.Score = score;
                    scoreData.Projection = projection;
                    scoreData.Impact = impact;
                    scoreData.MeetExceedValue = meetExceedValue;
                    scoreData.AssessmentDesc = assessmentDesc;
                    scoreData.ParentAssessmentTypeId = parentAssessmentTypeId;
                    scoreData.AssessmentId = assessmentId;
                    scoreData.AssessmentTypeId = assessmentTypeId;
                    scoreData.AssessmentCode = assessmentCode;
                }
                catch (Exception ex)
                {
                    throw new Exception("Check if field exists belongs to correct assessment", ex);
                }
                assessmentIndex++;

            }

            if (tempDetail != null)
            {
                tempDetail.AssessmentCount = assessmentIndex;
                results.Add(tempDetail);
                //if any student has less assessments than total assessments, then in order to create same header for all,
                //add assessment with null score from previous assessments. 
                results = GetFinalAssessmentAfterMergingMissingAssessments(results);
            }
            reader.Close();
            connection.Close();
            return results;
        }

        private void SetHeirarchicalAssessmentScoreData(out decimal? score, out decimal? projection, out decimal? impact, out decimal? percentile, out int? meetExceedValue, out decimal? growth, out int? parentAssessmentTypeId, out int assessmentId, out int assessmentTypeId, out string assessmentDesc, out string assessmentCode, bool viewScaledScore, SqlDataReader reader)
        {
            score = (reader["Score"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Score"]));
            projection = (reader["Projection"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Projection"]));
            impact = (reader["Impact"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(viewScaledScore ? RoundingType.SiteDefaultDecimalPlace : RoundingType.Default, Convert.ToDecimal(reader["Impact"]));
            percentile = (reader["DistrictPercentile"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["DistrictPercentile"]));
            meetExceedValue = (reader["MeetExceedValue"] == DBNull.Value) ? (int?)null : Convert.ToInt32(reader["MeetExceedValue"]);
            growth = (reader["GrowthCalc"] == DBNull.Value) ? (decimal?)null : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["GrowthCalc"]));
            parentAssessmentTypeId = reader["ParentAssessmentTypeId"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ParentAssessmentTypeId"]);
            assessmentId = Convert.ToInt32(reader["AssessmentId"]);
            assessmentTypeId = Convert.ToInt32(reader["AssessmentTypeId"]);
            assessmentDesc = reader["AssessmentDesc"] == DBNull.Value ? null : reader["AssessmentDesc"].ToString();
            assessmentCode = reader["AssessmentCode"] == DBNull.Value ? null : reader["AssessmentCode"].ToString();
        }


        internal AssessmentMEPerc GetMEPercentForAssessment(int districtId, int assementId, TIRDetailReportParameter detailReportParameter)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("spGetHeirarchicalReportMEPercentage", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@DistrictId", districtId));
            command.Parameters.Add(new SqlParameter("@SubjectId", detailReportParameter.Subject));
            command.Parameters.Add(new SqlParameter("@SchoolYearId", detailReportParameter.Year));
            command.Parameters.Add(new SqlParameter("@AssessmentTypeId", detailReportParameter.AssessmentTypeId));
            command.Parameters.Add(new SqlParameter("@TeacherId", detailReportParameter.Teacher));
            command.Parameters.Add(new SqlParameter("@GradeLevel", detailReportParameter.Grade));
            command.Parameters.Add(new SqlParameter("@ClassID", detailReportParameter.ClassId));
            command.Parameters.Add(new SqlParameter("@SchoolTermId", detailReportParameter.InputTermId));
            command.Parameters.Add(new SqlParameter("@Race", detailReportParameter.Race == -1 ? (int?)null : detailReportParameter.Race));
            command.Parameters.Add(new SqlParameter("@Gender", detailReportParameter.Gender == -1 ? (int?)null : detailReportParameter.Gender));
            command.Parameters.Add(new SqlParameter("@FrlIndicator", detailReportParameter.FrlIndicator));
            command.Parameters.Add(new SqlParameter("@IepIndicator", detailReportParameter.IEPIndicator));
            command.Parameters.Add(new SqlParameter("@LepIndicator", detailReportParameter.LEPIndicator));
            command.Parameters.Add(new SqlParameter("@Hispanic", detailReportParameter.Hispanic));

            SqlDataReader reader = command.ExecuteReader();

            AssessmentMEPerc tempAssessmentME = null;

            while (reader.Read())
            {
                tempAssessmentME = new AssessmentMEPerc();
                tempAssessmentME.AssessmentId = assementId;
                tempAssessmentME.MeetExceedPerc = reader["MeetExceedPerc"] == DBNull.Value ? (decimal)0.0 : HelperService.GetRoundedValue(RoundingType.SiteDefaultDecimalPlace, Convert.ToDecimal(reader["MeetExceedPerc"]));
                tempAssessmentME.MeetExceedCategory = reader["MeetExceedCategory"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["MeetExceedCategory"]);
                tempAssessmentME.SchoolTermId = (int)detailReportParameter.InputTermId;
                tempAssessmentME.AssessmentTypeId = detailReportParameter.AssessmentTypeId;
            }

            reader.Close();
            connection.Close();
            return tempAssessmentME;
        }
        public IEnumerable DropDownDataSchool(string selectedItem, int userId, int schoolYearId, bool showDefault)
        {
            var schools = (
                            from school in entities.tblSchools
                            join schoolUser in entities.tblUserSchools on school.SchoolId equals schoolUser.SchoolId
                            where schoolUser.UserId == userId && schoolUser.SchoolYearId == schoolYearId
                            orderby school.SchoolDesc
                            select new { SchoolId = school.SchoolId, SchoolDesc = school.SchoolDesc }
                          ).ToList();

            if (showDefault)
            {
                IEnumerable<SelectListItem> dropDownDefault = Enumerable.Repeat(
                       new SelectListItem { Value = "-1", Text = "--No School Found--" }, count: 1);
                if (schools.Count > 0)
                {
                    dropDownDefault = Enumerable.Repeat(
                      new SelectListItem { Value = "-1", Text = "--All--" }, count: 1);
                }
                return dropDownDefault.Concat(new MultiSelectList(
                       schools,
                       "SchoolId", "SchoolDesc", new string[] { selectedItem }));
            }
            else
            {
                IEnumerable<SelectListItem> dropDownDefault = new MultiSelectList(
                      schools,
                      "SchoolId", "SchoolDesc", new string[] { selectedItem });
                return dropDownDefault;
            }
        }
        public IEnumerable GetSchoolTerms(TIRDetailReportParameter detailReportParameter)
        {
            var terms = (from a in entities.tblAssessments
                         join st in entities.tblSchoolTerms on a.SchoolTermId equals st.SchoolTermId
                         join ass in entities.tblAssessmentScores on a.AssessmentId equals ass.AssessmentId
                         where a.AssessmentTypeId == detailReportParameter.AssessmentTypeId && a.SchoolYearId == detailReportParameter.Year
                               && a.SubjectId == detailReportParameter.Subject
                         orderby st.OrderBy descending
                         select new { SchoolTermId = a.SchoolTermId, SchoolTermDesc = st.SchoolTermDesc }
                        ).Distinct().ToList();

            return new MultiSelectList(
                   terms,
                   "SchoolTermId", "SchoolTermDesc", new int?[] { detailReportParameter.InputTermId });
        }

        public void IsChildAssessmentsExists(List<AssessmentMEPerc> assessmentMEPercList)
        {
            string condition = string.Empty;

            for (int i = 0; i < assessmentMEPercList.Count; i++)
            {
                if (i == 0)
                {
                    condition = "a.SchoolTermId =" + assessmentMEPercList[i].SchoolTermId + " && at.ParentAssessmentTypeId=" + assessmentMEPercList[i].AssessmentTypeId;
                }
                else
                {
                    condition += " || at.ParentAssessmentTypeId=" + assessmentMEPercList[i].AssessmentTypeId;
                }
            }

            var query = entities.tblAssessmentTypes
                                .Join(entities.tblAssessments, at => at.AssessmentTypeId, a => a.AssessmentTypeId, (at, a) => new { at, a })
                                .Where(condition)
                                .Select(x => new { ParentAssessmentTypeId = x.at.ParentAssessmentTypeId, SchoolTermId = x.a.SchoolTermId })
                                .Distinct();
            var results = query.ToList();
            if (results.Count > 0)
            {
                foreach (var q in assessmentMEPercList)
                {
                    var hasChild = results.Exists(a => a.ParentAssessmentTypeId == q.AssessmentTypeId && a.SchoolTermId == q.SchoolTermId);
                    q.IsChildExist = hasChild;
                }
            }
        }

        public List<HeirarchicalTIRDetail> GetFinalAssessmentAfterMergingMissingAssessments(List<HeirarchicalTIRDetail> existingAssessmentDetails)
        {
            // to get assessments with maximum number of assessments on top
            existingAssessmentDetails = existingAssessmentDetails.OrderByDescending(x => x.AssessmentCount).ToList();
            int maxAssessmentCount = existingAssessmentDetails.Max(x => x.AssessmentCount);
            HeirarchicalTIRDetail previousRecord = null;
            existingAssessmentDetails.ForEach(record =>
            {
                if (previousRecord != null && record.ScoreDataCollection.Count < maxAssessmentCount)
                {
                    previousRecord.ScoreDataCollection.ForEach(prevRecord =>
                    {
                        var assessmentExist = record.ScoreDataCollection.Exists(sa => sa.AssessmentTypeId == prevRecord.AssessmentTypeId);
                        if (!assessmentExist)
                        {
                            var newScoreData = new HeirarchicalScoreData();
                            newScoreData.AssessmentTypeId = prevRecord.AssessmentTypeId;
                            newScoreData.AssessmentCode = prevRecord.AssessmentCode;
                            record.ScoreDataCollection.Add(newScoreData);
                        }
                    });
                }
                previousRecord = record;
            });
            //sort by AssessmentTypeId to get merged and old records in order.
            existingAssessmentDetails.ForEach(record =>
            {
                var sortedScoreDataList = record.ScoreDataCollection.OrderBy(x => x.AssessmentTypeId).ToList();
                record.ScoreDataCollection = sortedScoreDataList;
            });
            return existingAssessmentDetails;
        }
    }
}
