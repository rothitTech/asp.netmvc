    
-- =============================================            
-- Author:  <Author,,Name>            
-- Create date: <Create Date,,>            
-- Description: <Description,,>      
    
-- Modified by: Hardev Gangwar      
 -- Modified dt: 09/12/2014                
-- Description: if class ID is not -1 then filter Teacher Impact Detail Assessment on class id     
          
-- spGetTeacherImpactDetailAssessmentList_New 1,3,1,17,5,28       
-- =============================================            
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetailAssessmentList]             
   @SubjectId int,            
   @SchoolYearId int,            
   @AssessmentTypeId int,            
   @TeacherId int,            
   @GradeLevel int,        
   @ClassID  INT        
AS          
        
-- Use Table Variable for class filter        
DECLARE @tblTempClassID TABLE      
(      
ID INT IDENTITY(1,1),      
ClassID int      
)         
          
IF @ClassID = -1        
BEGIN           
INSERT INTO @tblTempClassID(ClassID)           
SELECT c.ClassId              
  FROM tblClass c              
   join tblClassTeacher ct ON c.ClassId = ct.ClassId              
   join tblUser u ON ct.UserId = u.UserId              
  WHERE c.SchoolYearId = @SchoolYearId              
  and c.SubjectId = @SubjectId               
  and u.UserId = @TeacherId           
 END          
  ELSE          
  BEGIN            
  INSERT INTO @tblTempClassID(ClassID)          
  SELECT @ClassID        
  END        
          
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
where ClassId in ( SELECT ClassID FROM  @tblTempClassID)            
and a.AssessmentTypeId = @AssessmentTypeId            
and a.SchoolYearId = @SchoolYearId             
and a.SubjectId = @SubjectId            
order by ST.OrderBy            
END 