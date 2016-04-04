-- =============================================  
-- Author:  Vikrant Arya  
-- Create date: 06/10/2014      
-- Modified dt:   
-- Description: Proc to get the Meet or Exceed Percentage for different school term.       
-- =============================================  

-- Modified by: Vikrant Arya                   
-- Modified date : 11/21/2014                    
-- Description : Filter student against the selected report filters


-- Modified by: Sachin Gupta                   
-- Modified date : 11/25/2014                    
-- Description : Fixed Issue related to M/E Total was not coming correct for F&P assessment.
-- =========================================================================================   
-- EXEC [spGetMeetOrExceedPerc] 1,1,3,1,469,2,-1,3  

--Drop Proc if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='P' AND name = 'spGetMeetOrExceedPerc')
BEGIN
	DROP PROCEDURE spGetMeetOrExceedPerc
END 
GO

CREATE PROCEDURE [dbo].[spGetMeetOrExceedPerc]   
	@DistrictId INT,
	@SubjectId INT,  
	@SchoolYearId INT,  
	@AssessmentTypeId INT,  
	@TeacherId INT,  
	@GradeLevel INT,       
	@ClassId  INT,
	@SchoolTermId INT = NULL  
AS   
	DECLARE @StartRange DECIMAL (6,3)
	
	DECLARE @tblTempClass TABLE  
	(            
		ID INT IDENTITY(1,1),        
		ClassID int  
	)  
	
	DECLARE @tblTempStudent table
	(
		StudentId int not null
	)
	
	DECLARE @tblTempSchoolTerm TABLE  
	(            
		ID INT IDENTITY(1,1),        
		SchoolTermID int  
	)
	
	DECLARE @AssessmentType VARCHAR(20)
	Select @AssessmentType = AssessmentTypedesc from tblAssessmentType where AssessmentTypeId = @AssessmentTypeId

	--Execute Report filter stored proc to get the filter students
	IF NOT EXISTS(SELECT [name] FROM tempdb.sys.tables WHERE [name] like '#tblTempStudents%')
	BEGIN
		INSERT INTO @tblTempStudent (StudentId)
		SELECT S.STUDENTID FROM tblStudent S 
		INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
		WHERE SSY.SchoolYearId = @schoolYearId
	END
	ELSE
	BEGIN
		INSERT INTO @tblTempStudent (StudentId)
		SELECT StudentId FROM #tblTempStudents
	END
		  
	IF @ClassId = -1              
	BEGIN  
		INSERT INTO @tblTempClass(ClassID)  
		SELECT 
			c.ClassId     
		FROM tblClass c     
		JOIN tblClassTeacher ct ON c.ClassId = ct.ClassId     
		JOIN tblUser u ON ct.UserId = u.UserId     
		WHERE c.SchoolYearId = @SchoolYearId     
		AND c.SubjectId = @SubjectId      
		AND u.UserId = @TeacherId  
	END  
	ELSE  
	BEGIN   
		INSERT INTO @tblTempClass(ClassID)                
		SELECT @ClassId              
	END                
	
	IF (@SchoolTermId IS NULL)
	BEGIN
		INSERT INTO @tblTempSchoolTerm(SchoolTermID)
		SELECT SchoolTermId FROM tblSchoolTerm
	END
	ELSE
	BEGIN
		INSERT INTO @tblTempSchoolTerm(SchoolTermID)
		SELECT @SchoolTermId
	END
	
	SELECT    
		@StartRange = ac.StartRange
	FROM tblAssessmentWeighting aw  
	INNER JOIN tblAssessmentMeetExceedCriteria ac ON aw.AssessmentWeightingId = ac.AssessmentWeightingId  
	WHERE DistrictId = @DistrictId AND AssessmentTypeId = @AssessmentTypeId   
	AND SchoolYearId = @SchoolYearId AND SubjectId = @SubjectId
	
	BEGIN  

		SELECT 
			SchoolTermId
			,ROUND(CAST(SUM (Impact)/
						CAST(SUM(CASE WHEN (Impact IS NULL) THEN 0 ELSE 1 END) AS DECIMAL) * 100 AS DECIMAL(6,3)
						), 1)  AS MeetOrExceedPerc 
		 FROM 
		 (	
			    SELECT DISTINCT
				 s.StudentId
				,a.SchoolTermId as SchoolTermId
				,CASE(@AssessmentType) WHEN 'F & P' THEN
					CASE WHEN ass.ScaledScoreProjDif IS NULL 
						THEN NULL ELSE (CASE WHEN ass.ScaledScoreProjDif >= @StartRange THEN 1 ELSE 0 END) 
					END 
				 ELSE
				    CASE WHEN ass.ScoreProjDif IS NULL 
				      THEN NULL ELSE (CASE WHEN ass.ScoreProjDif >= @StartRange THEN 1 ELSE 0 END) 
				    END 
				 END as Impact           
	  		FROM tblStudent s  
			JOIN tblClassStudent cs ON s.StudentId = cs.StudentId  
			JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId  
			JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId  
			JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId  
			WHERE ClassId in (SELECT ClassID FROM  @tblTempClass)  
			AND a.AssessmentTypeId = @AssessmentTypeId  
			AND a.SchoolYearId = @SchoolYearId   
			AND a.SubjectId = @SubjectId  
			AND ass.GradeLevel = @GradeLevel  
			AND a.SchoolTermId IN (SELECT SchoolTermID FROM @tblTempSchoolTerm)
			AND s.StudentId  IN (SELECT studentId FROM @tblTempStudent)
		) AS SchoolTermData
		Group BY SchoolTermId
  END