USE [dbTIR]
GO
/****** Object:  StoredProcedure [dbo].[spGetMeetOrExceedPerc]    Script Date: 1/13/2015 3:34:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================  
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Renamed column AssessmentTypeDesc to AssessmentCode (US-224)    
-- =============================================  

ALTER PROCEDURE [dbo].[spGetMeetOrExceedPerc]   
	@DistrictId INT,
	@SubjectId INT,  
	@SchoolYearId INT,  
	@AssessmentTypeId INT,  
	@TeacherId INT,  
	@GradeLevel INT,       
	@ClassId  INT,
	@tblTempStudents varchar(20),	
	@SchoolTermId INT = NULL
AS   
	DECLARE @StartRange DECIMAL (6,3)
	
	DECLARE @tblTempClass TABLE  
	(            
		ID INT IDENTITY(1,1),        
		ClassID int  
	)  
	
	
	DECLARE @tblTempSchoolTerm TABLE  
	(            
		ID INT IDENTITY(1,1),        
		SchoolTermID int  
	)
	
	DECLARE @AssessmentCode VARCHAR(20)
	Select @AssessmentCode = AssessmentCode from tblAssessmentType where AssessmentTypeId = @AssessmentTypeId
				  
	IF @ClassId = -1              
	BEGIN  
		INSERT INTO @tblTempClass(ClassID)  
		SELECT 
			c.ClassId     
		FROM tblClass c     
		JOIN tblClassTeacher ct ON c.ClassId = ct.ClassId     
		JOIN tblUser u ON ct.UserId = u.UserId     
		WHERE c.SchoolYearId = @SchoolYearId     
		--AND c.SubjectId = @SubjectId      
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
				,CASE WHEN ass.ScoreProjDif IS NULL 
				      THEN NULL 
					  ELSE (CASE WHEN ass.ScoreProjDif >= @StartRange THEN 1 ELSE 0 END) 
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
			AND s.StudentId  IN (SELECT studentId FROM #@tblTempStudents)
		) AS SchoolTermData
		Group BY SchoolTermId
  END
