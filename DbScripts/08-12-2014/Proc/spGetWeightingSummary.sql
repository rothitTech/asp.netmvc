-- =============================================    
-- Author:  Vikrant Arya    
-- Create date: 7th Aug 2014    
-- Description: Proc to get the Weight summary, which varies on Subject, Grade, Assement Type    
-- =============================================    
--EXEC  [spGetWeightingSummary] @districtId = 1, @schoolYearId=3

CREATE PROCEDURE [dbo].[spGetWeightingSummary]    
	@districtId int,    
	@schoolYearId int    
AS    
BEGIN    
	SELECT 
		 agw.Grade AS Grade
		,at.AssessmentTypeDesc AS AssessmentType
		,sub.SubjectDesc AS ClassSubject
		,aw.AssessmentTypeId
		,agw.Weighting
		,aw.SubjectId
	FROM tblAssessmentWeighting aw 
	JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId
	JOIN tblSubject sub ON aw.SubjectId = sub.SubjectId    
	JOIN tblAssessmentType at ON at.AssessmentTypeId = aw.AssessmentTypeId    
	WHERE
		aw.DistrictId = @districtId AND aw.SchoolYearId = @schoolYearId
	GROUP BY 
		agw.Grade, aw.SubjectId, aw.AssessmentTypeId, at.AssessmentTypeDesc, sub.SubjectDesc, agw.Weighting
	ORDER BY 
		agw.Grade, aw.subjectId, agw.Weighting DESC    
END 