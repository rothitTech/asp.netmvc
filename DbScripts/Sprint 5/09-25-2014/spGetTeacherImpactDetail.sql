  
  
      
-- =============================================                    
-- Author:  Henry Schwenk                   
-- Create date: <Create Date,,>                 
-- Modified dt: 7/29/14 (Vikrant)                   
-- Description: spGetTeacherImpactDetail                  
                
-- Modified by: Sachin Gupta                
-- Modified date : 01/09/2014                
-- Changes done as per US-93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff                 
 -- should also be null )          
         
 -- Modified by: Hardev Gangwar        
 -- Modified dt: 09/12/2014                  
-- Description: if class ID is not -1 then filter TeacherImpactDetail on class id            
            
 -- Exec spGetTeacherImpactDetail_US116 2,3,3,17,5,1,-1    
                
-- =============================================                    
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetail]                     
   @SubjectId int,                    
   @SchoolYearId int,                    
   @AssessmentTypeId int,                    
   @TeacherId int,                    
   @GradeLevel int,                  
   @Flag bit  ,            
   @ClassID  INT                   
AS              
DECLARE @AssessmentType  varchar(20)

SELECT  @AssessmentType = AssessmentTypeDesc FROM tblassessmenttype where AssessmentTypeId= @AssessmentTypeId

-- Use Table Variable for class filter          
DECLARE @tblTempClassID table        
(        
ID INT IDENTITY(1,1),        
ClassID int        
)           
            
IF @ClassID = -1          
BEGIN             
INSERT INTO @tblTempClassID(ClassID)             
select c.ClassId                
  from tblClass c                
   join tblClassTeacher ct on c.ClassId = ct.ClassId                
   join tblUser u on ct.UserId = u.UserId                
  where c.SchoolYearId = @SchoolYearId                
  and c.SubjectId = @SubjectId                 
  and u.UserId = @TeacherId             
 END            
  ELSE            
  BEGIN              
  INSERT INTO @tblTempClassID(ClassID)            
  SELECT @ClassID          
  END            
            
                  
BEGIN                    
 IF(@Flag = 'true')                  
 Begin                  
  select s.StudentId,                     
   s.FirstName + ' ' + s.LastName as StudentName,                     
   a.AssessmentDesc,                     
   ass.ScaledScoreCalc,                     
   ass.ScaledProjCalc as Projection,                     
   ass.ScaledScoreProjDif as GrowthCalc,     
   (CASE(@AssessmentType) WHEN 'F & P' THEN ass.GrowthCalc Else ass.DistrictPercentile END ) as  DistrictPercentile ,                
 --  ass.DistrictPercentile as DistrictPercentile,                    
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
  where ClassId in (SELECT ClassID FROM  @tblTempClassID)                    
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
   ass.Projection as Projection,                     
   ass.ScoreProjDif as GrowthCalc,    
   (CASE(@AssessmentType) WHEN 'F & P' THEN ass.GrowthCalc Else ass.DistrictPercentile END ) as  DistrictPercentile ,                  
  -- ass.DistrictPercentile as DistrictPercentile,                    
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
  where ClassId in (SELECT ClassID FROM  @tblTempClassID)                    
  and a.AssessmentTypeId = @AssessmentTypeId                    
  and a.SchoolYearId = @SchoolYearId                     
  and a.SubjectId = @SubjectId                    
  -- HFS: 6/19/14 Added to limit detail report to a single grade level                    
  and ass.GradeLevel = @GradeLevel                    
  order by s.StudentId, st.OrderBy                    
 End                   
END 