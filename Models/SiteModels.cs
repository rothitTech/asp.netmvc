using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using EDS.Models;
using EDS;

namespace EDS
{
    public partial class tblUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<CheckBoxes> Schools { get; set; }
        public string SelectedSchools { get; set; }
    }

    public class tblUserExt : tblUser
    {
        public string RoleDesc { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }
        public int SchoolYearId { get; set; }
        public List<SchoolClass> SchoolClasses { get; set; }
    }
}

namespace EDS.Models
{
    public class SiteModels
    {
        public List<SchoolClass> SchoolClasses { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Student> Students { get; set; }
        public DropDownData DropDown { get; set; }
        public List<Announcement> Announcements { get; set; }
        //public List<KnowledgeBase> KnowledgeBaseItems { get; set; }
    }

    public class SchoolClass
    {
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int SchoolClassId { get; set; }
        public string ClassDesc { get; set; }
        public int SchoolYear { get; set; }
        public string SchoolYearDesc { get; set; }
        public string SchoolDesc { get; set; }
        public int TeachersCount { get; set; }
        public int StudentsCount { get; set; }
    }

    public class Teacher
    {
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SchoolDesc { get; set; }
    }

    public class Student
    {
        public int DistrictId { get; set; }
        public string DistrictDesc { get; set; }
        public int SchoolId { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public short? GradeLevel { get; set; }
        public string StateId { get; set; }
        public string LocalId { get; set; }
        public string HomeSchool { get; set; }

        public int? LineageId { get; set; }
        public int? RaceId { get; set; }
        public int? GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? HomeLanguageId { get; set; }
        public int? NativeLanguageId { get; set; }
        public int CreateUser { get; set; }
        public int ChangeUser { get; set; }
        
    }

    public class StudentExt : Student
    {
        public int StudentSchoolYearId { get; set; }
        public int SchoolYearId { get; set; }
        public string SchoolYearDesc { get; set; }
        public int ServingSchoolId { get; set; }
        public bool LepIndicator { get; set; }
        public bool IepIndicator { get; set; }
        public bool FrlIndicator { get; set; }
        public bool Hispanic { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime CreateDateTime { get; set; }
        public List<SchoolClass> StudentClasses { get; set; }
        public List<SchoolYear> SchoolYears { get; set; }
        public DropDownData DropDown { get; set; }
        public int SchoolYear { get; set; }
    }

    public class DropDownData
    {
        public YearDropDown Year { get; set; }
        public DistrictDropDown District { get; set; }
        public SchoolDropDown School { get; set; }
        public TeacherDropDown Teacher { get; set; }
        public ClassDropDown SchoolClass { get; set; }

        public LineageDropDown Lineage { get; set; }
        public GradeDropDown Grade { get; set; }
        public GenderDropDown Gender { get; set; }
        public RaceDropDown Race { get; set; }
        public LanguageDropDown HomeLanguage { get; set; }
        public LanguageDropDown NativeLanguage { get; set; }

    }

    public class DropDownIdName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string ToString()
        {
            return string.Format("{0},{1}", LastName, FirstName);
        }
    }

    public class DropDownIdDesc
    {
        public int Id { get; set; }
        public string Desc { get; set; }
    }

    public class YearDropDown
    {
        private readonly List<DropDownIdName> _years;

        public YearDropDown(List<DropDownIdName> years)
        {
            _years = years;
        }

        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> YearItems
        {
            get
            {
                var result = _years.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
                //return DefaultItem.Concat(result);
                return result;
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = "--All--"
                }, count: 1);
            }
        }
    }

    public class DistrictDropDown
    {
        private readonly List<DropDownIdName> _districts;

        public DistrictDropDown(List<DropDownIdName> districts)
        {
            _districts = districts;
        }

        public int SelectedDistrict { get; set; }

        public IEnumerable<SelectListItem> DistrictItems
        {
            get
            {
                var result = _districts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
                return DefaultItem.Concat(result);
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = "--All--"
                }, count: 1);
            }
        }
    }

    public class SchoolDropDown
    {
        private readonly List<DropDownIdName> _schools;
        private readonly bool _includeDefaultItem;
        private readonly string _defultText;
        private readonly string _defultValue;

        public SchoolDropDown(List<DropDownIdName> schools, bool includeDefaultItem = true, string defultText = "--All--", string defaultValue = "-1")
        {
            _schools = schools;
            _includeDefaultItem = includeDefaultItem;
            _defultText = defultText;
            _defultValue = defaultValue;
        }

        public int SelectedSchool { get; set; }

        public IEnumerable<SelectListItem> SchoolItems
        {
            get
            {
                if (_includeDefaultItem)
                {
                    return DefaultItem.Concat(_schools.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));
                }
                else
                {
                    return _schools.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
                }
                //return DefaultItem.Concat(_schools.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = _defultValue,
                    Text = _defultText
                }, count: 1);
            }
        }
    }

    public class TeacherDropDown
    {
        private readonly List<DropDownIdName> _teachers;
        private readonly string _defultText;
        private readonly string _defultValue;
        public TeacherDropDown(List<DropDownIdName> teachers, string defultText = "--All--", string defaultValue = "-1")
        {
            _teachers = teachers;
            _defultText = defultText;
            _defultValue = defaultValue;
        }

        public int SelectedTeacher { get; set; }

        public IEnumerable<SelectListItem> TeacherItems
        {
            get
            {
                if (_teachers.Count() > 1)
                {
                    return DefaultItem.Concat(_teachers.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));
                }
                else
                {
                    return _teachers.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
                }
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = _defultValue,
                    Text = _defultText
                }, count: 1);
            }
        }
    }

    public class ClassDropDown
    {
        private readonly List<DropDownIdName> _classes;
        private readonly string _defultText;
        private readonly string _defultValue;
        public ClassDropDown(List<DropDownIdName> classes, string defultText = "--All--", string defaultValue = "-1")
        {
            _classes = classes;
            _defultText = defultText;
            _defultValue = defaultValue;
        }

        public int SelectedClass { get; set; }

        public IEnumerable<SelectListItem> ClassItems
        {
            get
            {
                if (_classes.Count() > 1)
                {
                    return DefaultItem.Concat(_classes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));
                }
                else
                {
                    return _classes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
                }
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = _defultValue,
                    Text = _defultText
                }, count: 1);
            }
        }
    }

    public class GradeDropDown
    {
        private readonly List<DropDownIdDesc> _grades;

        public GradeDropDown(List<DropDownIdDesc> Grades)
        {
            _grades = Grades;
        }

        public int SelectedGrade { get; set; }

        public IEnumerable<SelectListItem> GradeItems
        {
            get
            {
                return _grades.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc });
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = ""
                }, count: 1);
            }
        }
    }

    public class LineageDropDown
    {
        private readonly List<DropDownIdDesc> _lineages;

        public LineageDropDown(List<DropDownIdDesc> Lineages)
        {
            _lineages = Lineages;
        }

        public int SelectedLineage { get; set; }

        public IEnumerable<SelectListItem> LineageItems
        {
            get
            {
                return DefaultItem.Concat(_lineages.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc }));
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = ""
                }, count: 1);
            }
        }
    }

    public class GenderDropDown
    {
        private readonly List<DropDownIdDesc> _genders;
        private readonly bool _includeDefaultItem;

        public GenderDropDown(List<DropDownIdDesc> Genders, bool includeDefaultItem = false)
        {
            _genders = Genders;
            _includeDefaultItem = includeDefaultItem;
        }

        public int SelectedGender { get; set; }

        public IEnumerable<SelectListItem> GenderItems
        {
            get
            {
                if (_includeDefaultItem)
                {
                    return DefaultItem.Concat(_genders.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc }));
                }
                else
                {
                    return _genders.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc });
                }
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = "--All--"
                }, count: 1);
            }
        }
    }

    public class RaceDropDown
    {
        private readonly List<DropDownIdDesc> _races;
        private readonly bool _includeDefaultItem;

        public RaceDropDown(List<DropDownIdDesc> Races, bool includeDefaultItem = false)
        {
            _races = Races;
            _includeDefaultItem = includeDefaultItem;
        }

        public int SelectedRace { get; set; }

        public IEnumerable<SelectListItem> RaceItems
        {
            get
            {
                if (_includeDefaultItem)
                {
                    return DefaultItem.Concat(_races.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc }));
                }
                else
                {
                    return EmptyDefaultItem.Concat(_races.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc }));
                }
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = "--All--"
                }, count: 1);
            }
        }


        public IEnumerable<SelectListItem> EmptyDefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = ""
                }, count: 1);
            }
        }

    }

    public class LanguageDropDown
    {
        private readonly List<DropDownIdDesc> _languages;

        public LanguageDropDown(List<DropDownIdDesc> Languages)
        {
            _languages = Languages;
        }

        public int SelectedLanguage { get; set; }

        public IEnumerable<SelectListItem> LanguageItems
        {
            get
            {
                return DefaultItem.Concat(_languages.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Desc }));
            }
        }

        public IEnumerable<SelectListItem> DefaultItem
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "-1",
                    Text = ""
                }, count: 1);
            }
        }
    }

    public class SiteUser
    {
        public int EdsUserId { get; set; }
        public string IdentityUserId { get; set; }
        public string IdentityUserName { get; set; }
        public string UserFullName { get; set; }
        public string Password { get; set; }
        public List<UserDisctrict> Districts { get; set; }
        public List<UserSchool> Schools { get; set; }
        public int? Role { get; set; }
        public string RoleDesc { get; set; }

        public bool isEdsAdministrator { get; set; }
        public bool isDataAdministrator { get; set; }
        public bool isAdministrator { get; set; }
        public bool isTeacher { get; set; }
    }

    public class UserDisctrict
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserSchool
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public int? UserId { get; set; }
        public int SchoolYearId { get; set; }
    }

    public class CheckBoxes
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public int? UserId { get; set; }
        public bool IsLocked { get; set; }
        public int? SchoolYearId { get; set; }
    }

    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDateTime { get; set; }
 
    }

    public class SchoolYear
    {
        public int SchoolYearId { get; set; }
        public int SchoolYearCode { get; set; }
        public string SchoolYearDesc { get; set; }
    }
}