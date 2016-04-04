--exec  spGetClassAssessmentScore 19,2,5,230,1
ALTER PROCEDURE [dbo].[spGetClassAssessmentScore]
@AssessmentTypeId INT,
@SubjectId INT,    
@SchoolYearId INT,     
@ClassId INT,
@SchoolTermId INT = NULL
AS

BEGIN
	DECLARE @tblTempSchoolTerms TABLE
	(
		SchoolTermId INT
	)

	IF(@SchoolTermId IS NULL)
	BEGIN
		INSERT INTO @tblTempSchoolTerms SELECT S.SchoolTermId FROM tblSchoolTerm S 
	END
	ELSE
	BEGIN
		INSERT INTO @tblTempSchoolTerms SELECT @SchoolTermId
	END

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
		stu.StudentId,     
		stu.LastName + ', ' + stu.FirstName AS StudentName,  
		stu.LastName,              
		scr.AssessmentScoreId,     
		scr.Score,    
		scr.Projection, 
		ass.AssessmentId,   
		ass.AssessmentTypeId,  
		ass.AssessmentDesc,   
		ass.AssessmentCode,
		ass.ReportTemplateId,
		ass.ScoreMin,
		ass.ScoreMax,
		ass.SchoolTermId,
		ass.GradeLevel
FROM 
	tblStudent stu 
	    JOIN tblStudentSchoolYear ssy ON stu.StudentId = ssy.StudentId
		JOIN tblClassStudent cs ON stu.StudentId = cs.StudentId  
		JOIN tblClass c ON cs.ClassId = c.ClassId 
		CROSS APPLY (
		SELECT 
			a.AssessmentId,   
			a.AssessmentTypeId,  
			a.AssessmentDesc,   
			at.AssessmentCode,
			at.ReportTemplateId,
			a.ScoreMin,
			a.ScoreMax,
			a.SchoolTermId,
			st.OrderBy,
			at.ParentAssessmentTypeId,
			ssy.GradeLevel
		FROM parentChildCTE pc
			JOIN tblAssessmentType at ON pc.AssessmentTypeId = at.AssessmentTypeId  
			JOIN tblAssessment a ON at.AssessmentTypeId = a.AssessmentTypeId 
			JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId
		WHERE parentId = @AssessmentTypeId
			AND a.SchoolYearId = @SchoolYearId     
			AND a.SubjectId = @SubjectId  
			AND a.SchoolTermId in (SELECT SchoolTermId FROM @tblTempSchoolTerms) 
			AND ssy.SchoolYearId = @SchoolYearId 
		) ass
		OUTER APPLY (
		SELECT
			s.AssessmentScoreId,     
			s.Score,    
			s.Projection
		FROM 
			tblAssessmentScore s
		WHERE
			s.AssessmentId = ass.AssessmentId
			AND s.StudentId = cs.StudentId
		) scr
WHERE c.ClassId = @ClassId

	ORDER BY 
		stu.StudentId -- Order by 'StudentId'
		,ass.OrderBy -- Order by column 'OrderBy' means for each school term there is an order defined.
		,ass.ParentAssessmentTypeId -- Order by 'ParentAssessmentId' means to keep ParentId at top from childerns for an assessment type.
		,ass.AssessmentTypeId	-- Order by AssessmentTypeId to get same order of accessments for all students.
END