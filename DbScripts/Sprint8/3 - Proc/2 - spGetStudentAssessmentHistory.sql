-- =============================================          
    
-- Author:  Henry Schwenk          
    
-- Create date: 2/26/14       
    
-- Modified dt: 7/29/14 (Vikrant)         
    
-- Description: GetStudentAssessmentHistory          
    
    
-- Modified by: Sachin Gupta    
-- Modified date : 01/09/2014    
-- Changes done as per US- 93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff     
-- should also be null )   
   
-- Modified by: Hardev Gangwar    
-- Modified date : 10/22/2014    
-- Changes done as per US-163 (Refactoring of SP and pass one more param DistrictId and return one more column MeetExceedValue for displaying arrow from tblAssessmentMeetExceedCriteria table)  
   
-- =============================================  
-- EXEC spGetStudentAssessmentHistory 92219, 0, 1

--Drop a proc if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetStudentAssessmentHistory')
BEGIN
	DROP PROCEDURE spGetStudentAssessmentHistory
END	
GO        
    
CREATE  PROCEDURE [dbo].[spGetStudentAssessmentHistory] 
@StudentId INT,   
@ViewScaledScore BIT,  
@DistrictId INT 
AS          
BEGIN  
	SET NOCOUNT ON; 
	SELECT 
		s.SubjectDesc,
		at.AssessmentTypeDesc, 
		a.AssessmentDesc,
		CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreCalc ELSE ass.Score END AS Score, 
		ass.[NationalPercentile] AS NationalPercentile, 
		ass.[DistrictPercentile] AS DistrictPercentile,        
		CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledProjCalc ELSE ass.Projection END AS Projection,    
		CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreProjDif ELSE ass.ScoreProjDif END AS Impact,   
		CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledGrowthCalc ELSE ass.GrowthCalc END AS Growth,     
		dbo.udfGetMeetExceedCriteriaValue(@DistrictId,a.SubjectId, a.SchoolYearId,a.AssessmentTypeId ,ass.ScoreProjDif)AS MeetExceedValue     
	FROM tblAssessmentScore ass          
    JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId          
    JOIN tblAssessmentType at ON a.AssessmentTypeId = at.AssessmentTypeId          
    JOIN tblSubject s ON a.SubjectId = s.SubjectId          
    JOIN tblSchoolYear sy ON a.SchoolYearId = sy.SchoolYearId          
    JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId          
    WHERE studentid = @StudentId          
    ORDER BY          
		a.SubjectId,           
		a.AssessmentTypeId,          
		sy.SchoolYear DESC,          
		st.OrderBy DESC         
END        