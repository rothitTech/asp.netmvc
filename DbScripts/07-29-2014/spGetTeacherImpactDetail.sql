-- =============================================    
-- Author:  <Author,,Name>    
-- Create date: <Create Date,,>    
-- Description: <Description,,>    
-- =============================================    
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetail]     
   @SubjectId int,    
   @SchoolYearId int,    
   @AssessmentTypeId int,    
   @TeacherId int,    
   @GradeLevel int,  
   @Flag bit     
AS    
BEGIN    
 IF(@Flag = 'true')  
 Begin  
  select s.StudentId,     
   s.FirstName + ' ' + s.LastName as StudentName,     
   a.AssessmentDesc,     
   ass.ScaledScoreCalc,     
   isnull(ass.ScaledProjCalc,0) as Projection,     
   isnull(ass.ScaledScoreProjDif,0) as GrowthCalc,     
   isnull(ass.DistrictPercentile, 0) as DistrictPercentile,    
   a.AssessmentId    
  from tblStudent s    
   join tblClassStudent cs    
    on s.StudentId = cs.StudentId    
   join tblAssessmentScore ass    
    on cs.StudentId = ass.StudentId    
   join tblAssessment a    
    on ass.AssessmentId = a.AssessmentId    
   join tblSchoolTerm st     
    on a.SchoolTermId = st.SchoolTermId    
  where ClassId in (    
    select c.ClassId    
    from tblClass c    
     join tblClassTeacher ct on c.ClassId = ct.ClassId    
     join tblUser u on ct.UserId = u.UserId    
    where c.SchoolYearId = @SchoolYearId    
    and c.SubjectId = @SubjectId     
    and u.UserId = @TeacherId     
    -- HFS: Removed to ignore the grade of a class in the detail report (i.e. allow classes with mutiple grades and/or ignore the class grade)    
    --and c.Grade = @GradeLevel    
    )    
  and a.AssessmentTypeId = @AssessmentTypeId    
  and a.SchoolYearId = @SchoolYearId     
  and a.SubjectId = @SubjectId    
  -- HFS: 6/19/14 Added to limit detail report to a single grade level    
  and ass.GradeLevel = @GradeLevel    
  order by s.StudentId, st.OrderBy    
 End  
 Else  
 Begin  
  select s.StudentId,     
   s.FirstName + ' ' + s.LastName as StudentName,     
   a.AssessmentDesc,     
   ass.Score,     
   isnull(ass.Projection,0) as Projection,     
   isnull(ass.ScoreProjDif,0) as GrowthCalc,     
   isnull(ass.DistrictPercentile, 0) as DistrictPercentile,    
   a.AssessmentId    
  from tblStudent s    
   join tblClassStudent cs    
    on s.StudentId = cs.StudentId    
   join tblAssessmentScore ass    
    on cs.StudentId = ass.StudentId    
   join tblAssessment a    
    on ass.AssessmentId = a.AssessmentId    
   join tblSchoolTerm st     
    on a.SchoolTermId = st.SchoolTermId    
  where ClassId in (    
    select c.ClassId    
    from tblClass c    
     join tblClassTeacher ct on c.ClassId = ct.ClassId    
     join tblUser u on ct.UserId = u.UserId    
    where c.SchoolYearId = @SchoolYearId    
    and c.SubjectId = @SubjectId     
    and u.UserId = @TeacherId     
    -- HFS: Removed to ignore the grade of a class in the detail report (i.e. allow classes with mutiple grades and/or ignore the class grade)    
    --and c.Grade = @GradeLevel    
    )    
  and a.AssessmentTypeId = @AssessmentTypeId    
  and a.SchoolYearId = @SchoolYearId     
  and a.SubjectId = @SubjectId    
  -- HFS: 6/19/14 Added to limit detail report to a single grade level    
  and ass.GradeLevel = @GradeLevel    
  order by s.StudentId, st.OrderBy    
 End   
END 