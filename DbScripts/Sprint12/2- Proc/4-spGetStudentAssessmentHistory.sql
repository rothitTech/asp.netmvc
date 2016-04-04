IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetStudentAssessmentHistory')
BEGIN
	DROP PROCEDURE spGetStudentAssessmentHistory
END	
GO
-- =============================================  
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename column AssessmentTypeDesc to AssessmentCode (US-224)    
-- =============================================  

CREATE PROCEDURE [dbo].[spGetStudentAssessmentHistory] 
@StudentId INT,   
@ViewScaledScore BIT,  
@DistrictId INT 
AS          
BEGIN  
	SET NOCOUNT ON; 
	SELECT 
		s.SubjectDesc,
		at.AssessmentCode, 
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
