    
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
    
                  
    
        
    
-- Modified by: Irabanta    
    
-- Modified date: 10/1/2014    
    
-- Description: story 140 to bring up LastName and default sorting on lastname. Also refactor @flag to @ViewScaledScore     
  
  
-- Exec spGetTeacherImpactDetail 2,3,3,17,5,1,-1 ,1  
-- Modified by: Hardev Gangwar    
    
-- Modified date: 10/07/2014    
    
-- Description: story 139 return MeetExceedCriteriaValue on the basisi of SubjectId, SchoolYearId,AssessmentTypeId and DistrictId  
    
    
-- =============================================    
    
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetail]     
    
   @SubjectId int,    
    
   @SchoolYearId int,    
    
   @AssessmentTypeId int,    
    
   @TeacherId int,    
    
   @GradeLevel int,         
    
   @ViewScaledScore bit,    
    
   @ClassID  INT ,    
     
   @DistrictId INT   
AS     
    
 DECLARE @AssessmentType  VARCHAR(20)      
    
     
    
 SELECT  @AssessmentType = AssessmentTypeDesc     
    
 FROM tblassessmenttype     
    
 WHERE AssessmentTypeId= @AssessmentTypeId      
    
       
    
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
    
   JOIN tblClassTeacher ct ON c.ClassId = ct.ClassId       
    
   JOIN tblUser u ON ct.UserId = u.UserId       
    
   WHERE c.SchoolYearId = @SchoolYearId       
    
   AND c.SubjectId = @SubjectId        
    
   AND u.UserId = @TeacherId    
    
 END    
    
 ELSE    
    
 BEGIN     
    
  INSERT INTO @tblTempClassID(ClassID)                  
    
  SELECT @ClassID                
    
 END                  
    
          
    
 BEGIN    
    
  SELECT s.StudentId,     
    
   s.FirstName + ' ' + s.LastName as StudentName,     
    
   a.AssessmentDesc,     
    
   CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreCalc ELSE ass.Score END as Score,    
    
   CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledProjCalc ELSE ass.Projection END as Projection,     
    
   CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreProjDif ELSE ass.ScoreProjDif END as Impact,           
    
   (CASE(@AssessmentType) WHEN 'F & P' THEN ass.GrowthCalc Else ass.DistrictPercentile END ) as  DistrictPercentile ,       
    
   a.AssessmentId,    
    
   s.LastName,    
    
   @AssessmentType as AssessmentType  , dbo.udfGetMeetExceedCriteriaValue(@DistrictId,@SubjectId,@SchoolYearId,@AssessmentTypeId,ass.ScoreProjDif)AS MeetExceedValue 
     
  FROM tblStudent s    
    
  JOIN tblClassStudent cs ON s.StudentId = cs.StudentId    
    
  JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId    
    
  JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId    
    
  JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId    
    
  WHERE ClassId in (SELECT ClassID FROM  @tblTempClassID)    
    
   AND a.AssessmentTypeId = @AssessmentTypeId    
    
   AND a.SchoolYearId = @SchoolYearId     
    
   AND a.SubjectId = @SubjectId    
    
   -- HFS: 6/19/14 Added to limit detail report to a single grade level    
    
   AND ass.GradeLevel = @GradeLevel    
    
  ORDER BY s.StudentId, st.OrderBy    
    
 END

