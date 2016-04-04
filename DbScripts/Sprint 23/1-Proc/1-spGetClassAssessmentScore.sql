--exec spGetClassAssessmentScore 11,7,3,229,3
ALTER PROCEDURE [dbo].[spGetClassAssessmentScore]
@AssessmentTypeId INT,
@SubjectId INT,    
@SchoolYearId INT,     
@ClassId INT,
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
		s.StudentId,     
		s.FirstName + ' ' + s.LastName AS StudentName,  
		s.LastName,   
		ass.AssessmentScoreId,     
		ass.Score,    
		ass.Projection,     			           
		a.AssessmentId,   
		a.AssessmentTypeId,  
		a.AssessmentDesc,   
		at.AssessmentCode,
		at.ReportTemplateId,
		a.ScoreMin,
		a.ScoreMax,
		a.SchoolTermId
	FROM tblStudent s    
	JOIN tblStudentSchoolYear ssy ON s.StudentId = ssy.StudentId
	JOIN tblClassStudent cs ON s.StudentId = cs.StudentId    
	JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId    
	JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId    
	JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId   
	JOIN tblAssessmentType at ON a.AssessmentTypeId = at.AssessmentTypeId  
	JOIN tblReportTemplate rt ON at.ReportTemplateId = rt.TemplateId
	JOIN tblClass c ON cs.ClassId = c.ClassId
	WHERE c.ClassId = @ClassId
	AND at.AssessmentTypeId IN (SELECT AssessmentTypeId FROM parentChildCTE WHERE parentId = @AssessmentTypeId)
	AND a.SchoolYearId = @SchoolYearId     
	AND a.SubjectId = @SubjectId  
	AND c.SubjectId = @SubjectId    
	AND ssy.SchoolYearId = @SchoolYearId
	AND a.SchoolTermId = @SchoolTermId 
	ORDER BY 
		s.StudentId -- Order by 'StudentId'
		,st.OrderBy -- Order by column 'OrderBy' means for each school term there is an order defined.
		,at.ParentAssessmentTypeId -- Order by 'ParentAssessmentId' means to keep ParentId at top from childerns for an assessment type.
		,a.AssessmentTypeId	-- Order by AssessmentTypeId to get same order of accessments for all students.
END