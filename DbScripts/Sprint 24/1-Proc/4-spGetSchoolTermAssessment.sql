--exec spGetSchoolTermAssessment 19,2,5,2
CREATE PROCEDURE [dbo].[spGetSchoolTermAssessment]
@AssessmentTypeId INT,
@SubjectId INT,    
@SchoolYearId INT,     
@SchoolTermId INT
AS

BEGIN
	;WITH parentChildCTE AS (
		SELECT  AssessmentTypeId, AssessmentTypeId AS parentId
		FROM tblAssessmentType
		WHERE ParentAssessmentTypeId IS NULL
		UNION ALL
		SELECT child.AssessmentTypeId, parent.parentId
		FROM tblAssessmentType child
		JOIN parentChildCTE parent
		ON parent.AssessmentTypeId = child.ParentAssessmentTypeId
	  )

	SELECT    
		a.AssessmentTypeId, 
		a.AssessmentId, 
		at.AssessmentCode,
		a.SchoolTermId,
		a.ScoreMin,
		a.ScoreMax
	FROM tblAssessment a      
	JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId   
	JOIN tblAssessmentType at ON a.AssessmentTypeId = at.AssessmentTypeId  
	WHERE a.SchoolTermId = @SchoolTermId
	AND at.AssessmentTypeId IN (SELECT AssessmentTypeId FROM parentChildCTE WHERE parentId = @AssessmentTypeId)
	AND a.SchoolYearId = @SchoolYearId     
	AND a.SubjectId = @SubjectId  
	ORDER BY 
		st.OrderBy -- Order by column 'OrderBy' means for each school term there is an order defined.
		,at.ParentAssessmentTypeId -- Order by 'ParentAssessmentId' means to keep ParentId at top from childerns for an assessment type.
		,a.AssessmentTypeId	-- Order by AssessmentTypeId to get same order of accessments for all students.
END