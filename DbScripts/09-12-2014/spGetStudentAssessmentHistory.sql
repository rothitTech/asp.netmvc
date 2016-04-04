-- =============================================        
  
-- Author:  Henry Schwenk        
  
-- Create date: 2/26/14     
  
-- Modified dt: 7/29/14 (Vikrant)       
  
-- Description: GetStudentAssessmentHistory        
  
  
-- Modified by: Sachin Gupta  
-- Modified date : 01/09/2014  
-- Changes done as per US- 93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff   
 -- should also be null )  
-- =============================================        
  
ALTER  PROCEDURE [dbo].[spGetStudentAssessmentHistory]        
  
 @StudentId int,      
  
 @showRawScale bit        
  
AS        
  
BEGIN        
  
 SET NOCOUNT ON;        
  
 IF(@showRawScale = 1)      
  
 Begin      
  
  select         
  
    s.SubjectDesc,         
  
    at.AssessmentTypeDesc,         
  
    a.AssessmentDesc,         
  
     ass.ScaledScoreCalc as Score,          
  
    ass.[NationalPercentile] as NationalPercentile,       
  
    ass.[DistrictPercentile] as DistrictPercentile,      
  
    ass.[ScaledProjCalc] as Projection,        
  
    ass.[ScaledScoreProjDif] as Impact,   
  
    ass.[ScaledGrowthCalc] as Growth         
  
          
  
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
  
    Score,     
  
    ass.[NationalPercentile] as NationalPercentile,       
  
    ass.[DistrictPercentile] as DistrictPercentile,         
  
    ass.[Projection] as Projection,        
  
    ass.[ScoreProjDif] as Impact,   
  
    ass.[GrowthCalc] as Growth    
  
          
  
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