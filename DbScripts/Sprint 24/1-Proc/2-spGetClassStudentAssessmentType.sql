--exec spGetClassStudentAssessmentType 19,2,5,230
ALTER PROCEDURE [dbo].[spGetClassStudentAssessmentType]
@AssessmentTypeId INT,
@SubjectId INT,    
@SchoolYearId INT,     
@ClassId INT,
@SchoolId INT
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
		s.StudentId,     
		s.FirstName + ' ' + s.LastName AS StudentName,  
		s.LastName,   
		a.AssessmentTypeId, 
		a.AssessmentId, 
		at.AssessmentCode,
		a.SchoolTermId,
		at.ReportTemplateId,
		a.ScoreMin,
		a.ScoreMax
	FROM tblStudent s    
	JOIN tblStudentSchoolYear ssy ON s.StudentId = ssy.StudentId
	JOIN tblClassStudent cs ON s.StudentId = cs.StudentId    
	JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId    
	JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId    
	JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId   
	JOIN tblAssessmentType at ON a.AssessmentTypeId = at.AssessmentTypeId  
	JOIN tblClass c ON cs.ClassId = c.ClassId
	WHERE c.ClassId = @ClassId
	AND at.AssessmentTypeId IN (SELECT AssessmentTypeId FROM parentChildCTE WHERE parentId = @AssessmentTypeId)
	AND a.SchoolYearId = @SchoolYearId     
	AND a.SubjectId = @SubjectId  
	AND c.SubjectId = @SubjectId    
	AND ssy.SchoolYearId = @SchoolYearId
	AND ssy.ServingSchoolId = @SchoolId
	ORDER BY 
		s.StudentId -- Order by 'StudentId'
		,st.OrderBy -- Order by column 'OrderBy' means for each school term there is an order defined.
		,at.ParentAssessmentTypeId -- Order by 'ParentAssessmentId' means to keep ParentId at top from childerns for an assessment type.
		,a.AssessmentTypeId	-- Order by AssessmentTypeId to get same order of accessments for all students.
END

