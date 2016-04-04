-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetailAssessmentList]   
   @SubjectId int,  
   @SchoolYearId int,  
   @AssessmentTypeId int,  
   @TeacherId int,  
   @GradeLevel int  
AS  
BEGIN  
select distinct a.AssessmentDesc, a.AssessmentId, st.OrderBy  
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
  --and c.Grade = @GradeLevel  
  )  
and a.AssessmentTypeId = @AssessmentTypeId  
and a.SchoolYearId = @SchoolYearId   
and a.SubjectId = @SubjectId  
order by ST.OrderBy  
END  