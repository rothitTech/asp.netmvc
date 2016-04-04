IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetWeightingSummary')
BEGIN
	DROP PROCEDURE spGetWeightingSummary
END	
GO
-- =============================================    
-- Author:  Vikrant Arya    
-- Create date: 7th Aug 2014    
-- Description: Proc to get the Weight summary, which varies on Subject, Grade, Assement Type    

-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename column AssessmentTypeDesc to AssessmentCode     
-- =============================================    

--EXEC  [spGetWeightingSummary] @districtId = 1, @schoolYearId=3


CREATE PROCEDURE [dbo].[spGetWeightingSummary]    
	@districtId int,    
	@schoolYearId int    
AS    
BEGIN    
	SELECT 
		 agw.Grade AS Grade
		,at.AssessmentCode AS AssessmentCode
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
		agw.Grade, aw.SubjectId, aw.AssessmentTypeId, at.AssessmentCode, sub.SubjectDesc, agw.Weighting
	ORDER BY 
		agw.Grade ASC, sub.SubjectDesc ASC, agw.Weighting DESC, at.AssessmentCode ASC    
END 

