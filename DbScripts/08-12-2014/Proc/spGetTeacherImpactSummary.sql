-- =============================================    
-- Author:  <Author,,Name>    
-- Create date: <Create Date,,>    
-- Description: <Description,,>    
-- =============================================    
ALTER PROCEDURE [dbo].[spGetTeacherImpactSummary]    
 @teacherId int,    
 @schoolId int,    
 @schoolYearId int    
AS    
BEGIN    
 declare @districtId int    
 set @districtId = (select districtId from tblSchool where SchoolId = @SchoolId)    
     
 select base.Grade, base.AssessmentType, base.ClassSubject,  base.AssessmentTypeId, agw.Weighting, base.Impact, base.SubjectId     
    
 from    
 (select s.DistrictId, a.SchoolYearId, sc.GradeLevel as Grade, a.SubjectId,  a.AssessmentTypeId, at.AssessmentTypeDesc as AssessmentType, sub.SubjectDesc as ClassSubject, sum(isnull(sc.ScaledScoreProjDif,0)) as Impact    
 from tblAssessment a    
 join tblAssessmentScore sc on sc.AssessmentId = a.AssessmentId   
-- and sc.SchoolID = @schoolId  --Code Uncommented for US 73  
 join tblSubject sub on a.SubjectId = sub.SubjectId    
 join tblAssessmentType at on at.AssessmentTypeId = a.AssessmentTypeId    
 join tblschool s on s.SchoolId = sc.SchoolID    
 join (select cs1.StudentId, c.Grade, c.SubjectId     
  from tblClassStudent cs1    
  join tblClass c on c.ClassId = cs1.ClassId    
  where     
   cs1.ClassId in (    
    select tc1.ClassId    
    from tblClassTeacher tc1    
    where tc1.UserId = @teacherId    
   )    
   and c.SchoolYearId =  @schoolYearId    
   ) as cs2     
    on cs2.StudentId = sc.StudentId     
 --   and cs2.Grade = sc.GradeLevel   --Code Uncommented for US 73  
    and cs2.SubjectId = a.SubjectId    
 where a.AssessmentId in (select AssessmentId from [dbo].[udfDistrictsLastAssessmentsTable](@districtId, @schoolYearId ))    
 group by s.DistrictId, a.SchoolYearId, sc.GradeLevel, a.SubjectId, a.AssessmentTypeId, at.AssessmentTypeDesc, sub.SubjectDesc) as base    
    
 join tblAssessmentWeighting aw on aw.DistrictId = base.DistrictId and aw.SchoolYearId = base.SchoolYearId and aw.AssessmentTypeId = base.AssessmentTypeId and aw.SubjectId = base.SubjectId    
 JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId and base.Grade = agw.Grade
 order by base.Grade, aw.subjectId, agw.Weighting desc    
END 