USE [dbTIR]
GO
/****** Object:  StoredProcedure [dbo].[spGetTeacherSummaryView]    Script Date: 1/19/2015 12:13:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--===================================================
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename AssessmentType to AssessmentCode and AssessmentTypeFullDescText to AssessmentTypeDesc

-- Author:  Sachin Gupta  
-- Modified date: 19/01/15  
-- Description: Add classId check to filter students for a class 
--================================================================================
--exec spGetTeacherSummaryView 17, 1, 3, -1, 1

ALTER PROCEDURE [dbo].[spGetTeacherSummaryView]   
	@TeacherId INT,      
	@SchoolId INT,      
	@SchoolYearId INT,
	@ClassId INT, 
	@ViewMeetExceedSummary BIT,
	@Race INT = NULL,
	@Gender INT = NULL,
	@FrlIndicator BIT = NULL,
	@IepIndicator BIT = NULL,
	@LepIndicator BIT = NULL,
	@Hispanic BIT = NULL
AS   
BEGIN
	DECLARE @Counter INT
			,@Count INT
			,@DistrictId INT
			,@SubjectId INT
			,@AssessmentTypeId INT
			,@Grade INT
			,@SchoolTermId INT
			,@MeetExceedPerc DECIMAL(6,3)
			,@Impact float
			,@Query VARCHAR(MAX)
			,@tblTempStudents varchar(20)
	
	DECLARE @tblTempTeacherSummary  TABLE
	(            
		 CounterId INT IDENTITY(1,1)        
		,Grade INT
		,AssessmentCode NVARCHAR(256)
		,ClassSubject NVARCHAR(256)
		,AssessmentTypeId INT
		,Weighting DECIMAL (6,3)
		,Impact float
		,SubjectId INT
		,NoOfStudent INT
		,Average  float     
		,AssessmentGradeWeightingId INT
		,MeetExceedPerc DECIMAL (6,3)
		,MeetExceedCategory INT
		,SchoolTermId INT
		,MeetExceedPoints DECIMAL (6,3)
		,IsAssessmentExist BIT
		,ReportTemplateId INT
		,AssessmentTypeDesc NVARCHAR(200)
		,SortCriteria1 VARCHAR(200)
		,SortCriteria2 VARCHAR(200)	
	)  
	
	DECLARE @tblTempMeetExceedPerc  TABLE
	(
		 SchoolTermId INT
		,MeetExceedPerc DECIMAL (6,3)
	)
	
	SET @DistrictId = (SELECT districtId FROM tblSchool WHERE SchoolId = @SchoolId)  

	-- Create and populate table used in spGetTeacherImpactSummary and spGetMeetOrExceedPerc
	SET @tblTempStudents ='tblTempStudents'
	CREATE TABLE #@tblTempStudents (StudentId int, PRIMARY KEY CLUSTERED (StudentId))
	INSERT INTO #@tblTempStudents SELECT S.STUDENTID FROM tblStudent S 
      INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
      WHERE SSY.SchoolYearId = @schoolYearId
      AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
      AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
      AND (@Hispanic IS NULL OR SSY.Hispanic = @Hispanic)
      AND (@Race IS NULL OR S.RaceId = @Race)
      AND (@Gender IS NULL OR S.GenderId = @Gender)	
	  AND S.STUDENTID IN 
	  (
		select distinct cs.StudentId
		from tblClass c
			join tblClassTeacher ct on ct.ClassId = c.classId
			join tblClassStudent cs on cs.ClassId = c.classId
		where c.SchoolYearId = @SchoolYearId
		and ct.UserId = @TeacherId    
		and c.ClassId = CASE WHEN @ClassId = -1 THEN c.ClassId  ELSE @ClassId END     
		)
		
	--Executing Teacher Impact Summary Proc
	INSERT INTO @tblTempTeacherSummary
	(Grade,AssessmentCode,ClassSubject,AssessmentTypeId,Weighting,
	 Impact,SubjectId,NoOfStudent,Average,AssessmentGradeWeightingId,SchoolTermId, SortCriteria1, SortCriteria2,IsAssessmentExist,ReportTemplateId,AssessmentTypeDesc )
	EXEC [spGetTeacherImpactSummary] @TeacherId, @SchoolId, @SchoolYearId, @ClassId, @ViewMeetExceedSummary, @tblTempStudents
	
	IF(@ViewMeetExceedSummary = 1)
	BEGIN 
		SET @Counter = 1
		SELECT @Count = COUNT(*) FROM @tblTempTeacherSummary
		
		WHILE( @Counter <= @Count)
		BEGIN
			SET @SubjectId = NULL
			SET @AssessmentTypeId = NULL
			SET @Grade = NULL
			SET @SchoolTermId = NULL
			SET @Impact = NULL
			SET @MeetExceedPerc = NULL
				
			SELECT 
				 @SubjectId = SubjectId
				,@AssessmentTypeId = AssessmentTypeId
				,@Grade = Grade
				,@SchoolTermId = SchoolTermId
				,@Impact = Impact
			FROM @tblTempTeacherSummary
			WHERE CounterId = 	@Counter
			
			--Exceuting Meet Exceed Percentage Proc
			INSERT INTO @tblTempMeetExceedPerc
			(SchoolTermId, MeetExceedPerc)
			EXEC [spGetMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
			@AssessmentTypeId, @TeacherId, @Grade, @ClassId, @tblTempStudents, @SchoolTermId

			SELECT TOP 1
				@MeetExceedPerc = MeetExceedPerc
			FROM @tblTempMeetExceedPerc
			WHERE MeetExceedPerc IS NOT NULL 
			ORDER by SchoolTermId DESC	
			
			--Updating the Meet Exceed Precentage and Meet Exceed Category			
			UPDATE @tblTempTeacherSummary
				SET MeetExceedPerc = @MeetExceedPerc
					 ,MeetExceedCategory = (SELECT [dbo].[udfGetMeetExceedCategory] (@DistrictId, @SubjectId, @SchoolYearId, @AssessmentTypeId, @Grade, @MeetExceedPerc ))     
			         ,MeetExceedPoints =  (SELECT [dbo].[udfGetMeetExceedPoint] (@DistrictId, @SubjectId, @SchoolYearId, @AssessmentTypeId, @Grade, @MeetExceedPerc ))   
			WHERE CounterId = 	@Counter	
			
			DELETE FROM @tblTempMeetExceedPerc
			SET @Counter = @Counter + 1
		END
	END
		
	DROP TABLE #@tblTempStudents

	SELECT  
		Grade
		,AssessmentCode
		,ClassSubject
		,AssessmentTypeId
		,Weighting
		,Impact
		,SubjectId
		,NoOfStudent
		,Average
		,AssessmentGradeWeightingId
		,MeetExceedPerc 
		,MeetExceedCategory
		,MeetExceedPoints
		,IsAssessmentExist
		,ReportTemplateId
		,AssessmentTypeDesc
	FROM @tblTempTeacherSummary
	
END	
