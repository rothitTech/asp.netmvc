--exec spGetHeirachicalAssessmentType 19,2,5,2
CREATE PROCEDURE [dbo].[spGetHeirachicalAssessmentType]
@AssessmentTypeId INT
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
		at.AssessmentTypeId, 
		at.AssessmentCode
	FROM tblAssessmentType at
	WHERE 
	at.AssessmentTypeId IN (SELECT AssessmentTypeId FROM parentChildCTE WHERE parentId = @AssessmentTypeId)
	ORDER BY 
	at.AssessmentTypeId	
END

