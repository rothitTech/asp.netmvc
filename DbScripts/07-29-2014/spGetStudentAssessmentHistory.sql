-- =============================================    
-- Author:  Henry Schwenk    
-- Create date: 2/26/14    
-- Description: GetStudentAssessmentHistory    
-- =============================================    
ALTER PROCEDURE [dbo].[spGetStudentAssessmentHistory]    
 @StudentId int,  
 @Flag bit    
AS    
BEGIN    
 SET NOCOUNT ON;    
 IF(@Flag = 'true')  
 Begin  
  select     
    s.SubjectDesc,     
    at.AssessmentTypeDesc,     
    a.AssessmentDesc,     
    isnull(ass.ScaledScoreCalc,0) as Score,    
    isnull(ass.[NationalPercentile],0) as NationalPercentile,    
    isnull(ass.[DistrictPercentile],0) as DistrictPercentile,    
    isnull(ass.[ScaledProjCalc],0) as Projection,    
    isnull(ass.[ScaledScoreProjDif],0) as Impact,    
    isnull(ass.[ScaledGrowthCalc],0) as Growth    
      
   from tblAssessmentScore ass    
    join tblAssessment a on ass.AssessmentId = a.AssessmentId    
    join tblAssessmentType at on a.AssessmentTypeId = at.AssessmentTypeId    
    join tblSubject s on a.SubjectId = s.SubjectId    
    join tblSchoolYear sy on a.SchoolYearId = sy.SchoolYearId    
    join tblSchoolTerm st on a.SchoolTermId = st.SchoolTermId    
   where studentid = @StudentId    
   order by     
    a.SubjectId,     
    a.AssessmentTypeId,    
    sy.SchoolYear desc,    
    st.OrderBy desc   
 END  
 ELSE  
 BEGIN  
  select     
    s.SubjectDesc,     
    at.AssessmentTypeDesc,     
    a.AssessmentDesc,     
    isnull(ass.Score,0) as Score,    
    isnull(ass.[NationalPercentile],0) as NationalPercentile,    
    isnull(ass.[DistrictPercentile],0) as DistrictPercentile,    
    isnull(ass.[Projection],0) as Projection,    
    isnull(ass.[ScoreProjDif],0) as Impact,    
    isnull(ass.[GrowthCalc],0) as Growth    
      
   from tblAssessmentScore ass    
    join tblAssessment a on ass.AssessmentId = a.AssessmentId    
    join tblAssessmentType at on a.AssessmentTypeId = at.AssessmentTypeId    
    join tblSubject s on a.SubjectId = s.SubjectId    
    join tblSchoolYear sy on a.SchoolYearId = sy.SchoolYearId    
    join tblSchoolTerm st on a.SchoolTermId = st.SchoolTermId    
   where studentid = @StudentId    
   order by     
    a.SubjectId,     
    a.AssessmentTypeId,    
    sy.SchoolYear desc,    
    st.OrderBy desc  
 END    
END 