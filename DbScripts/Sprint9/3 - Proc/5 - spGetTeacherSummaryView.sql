
-- =============================================  
-- Author:  Vikrant Arya  
-- Create date: 09/10/2014      
-- Modified dt:   
-- Description: Proc to execute the Teacher Impact Summary and Meet Exceed Criteria

-- =============================================  
-- Modified by :  Hardev Gangwar  
-- Modified date: 13/10/2014
-- Description: added MeetExceedPoints function which return the points value
-- =============================================  
-- Modified by: Sachin Gupta                 
-- Modified date : 10/16/2014                  
-- Description : Modified sp to allow conditional sorting on basis of ViewMeetExceedSummary flag as per US-160 


-- Modified by: Hardev Gangwar                
-- Modified date : 10/30/2014                  
-- Description : Modified sp to return one more column IsAssessmentExist as flag as per US-146 

-- Modified by: Vikrant Arya                   
-- Modified date : 11/19/2014                    
-- Description : Includes the report filters as per US-202

-- Modified by: Vikrant Arya                   
-- Modified date : 11/21/2014                    
-- Description : Filter student against the selected report filters
-- =============================================    
-- EXEC [dbo].[spGetTeacherSummaryView]  447, 1, 3,-1 , 1, NULL, 1



--Drop function if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='P' AND name = 'spGetTeacherSummaryView')
BEGIN
	DROP PROCEDURE spGetTeacherSummaryView
END 
GO

CREATE PROCEDURE [dbo].[spGetTeacherSummaryView]   
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
			,@Impact DECIMAL(6,3)
			,@Query VARCHAR(MAX)
	
	DECLARE @tblTempTeacherSummary  TABLE
	(            
		 CounterId INT IDENTITY(1,1)        
		,Grade INT
		,AssessmentType NVARCHAR(256)
		,ClassSubject NVARCHAR(256)
		,AssessmentTypeId INT
		,Weighting DECIMAL (6,3)
		,Impact DECIMAL (6,3)
		,SubjectId INT
		,NoOfStudent INT
		,Average  DECIMAL (6,3)     
		,AssessmentGradeWeightingId INT
		,MeetExceedPerc DECIMAL (6,3)
		,MeetExceedCategory INT
		,SchoolTermId INT
		,MeetExceedPoints DECIMAL (6,3)
		,IsAssessmentExist BIT
		,SortCriteria1 VARCHAR(200)
		,SortCriteria2 VARCHAR(200)
		
	)  
	
	DECLARE @tblTempMeetExceedPerc  TABLE
	(
		 SchoolTermId INT
		,MeetExceedPerc DECIMAL (6,3)
	)
	
	CREATE  TABLE #tblTempStudents 
	(
		studentId int not null
	)
          
	SET @Query = 'SELECT S.STUDENTID FROM tblStudent S 
				  INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
				  WHERE SSY.SchoolYearId = ' + CONVERT(VARCHAR(10), @schoolYearId) 

	IF(@LepIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.LepIndicator = ' + CONVERT(VARCHAR(10), @LepIndicator)
	END

	IF(@IepIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.IepIndicator = ' + CONVERT(VARCHAR(10), @IepIndicator)
	END

	IF(@FrlIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.FrlIndicator = ' +  CONVERT(VARCHAR(10), @FrlIndicator)
	END

	IF(@Hispanic IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.Hispanic = ' +  CONVERT(VARCHAR(10), @Hispanic)
	END

	IF(@Race IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND S.RaceId = ' +  CONVERT(VARCHAR(10), @Race)
	END

	IF(@Gender IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND S.GenderId = ' +  CONVERT(VARCHAR(10), @Gender)
	END

	--Retrieving students for the report filter
	INSERT INTO #tblTempStudents(studentId)
	EXEC (@Query)
		
	--Executing Teacher Impact Summary Proc
	INSERT INTO @tblTempTeacherSummary
	(Grade,AssessmentType,ClassSubject,AssessmentTypeId,Weighting,
	 Impact,SubjectId,NoOfStudent,Average,AssessmentGradeWeightingId,SchoolTermId, SortCriteria1, SortCriteria2,IsAssessmentExist )
	EXEC [spGetTeacherImpactSummary] @TeacherId, @SchoolId, @SchoolYearId, @ClassId, @ViewMeetExceedSummary
	
	IF(@ViewMeetExceedSummary = 1)
	BEGIN
		SET @DistrictId = (SELECT districtId FROM tblSchool WHERE SchoolId = @SchoolId)   
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
			@AssessmentTypeId, @TeacherId, @Grade, @ClassId, @SchoolTermId
			
			SELECT	TOP 1
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
	
	IF EXISTS(SELECT [name] FROM tempdb.sys.tables WHERE [name] like '#tblTempStudents%')
	BEGIN
		DROP TABLE #tblTempStudents
	END
	
	SELECT  
		Grade
		,AssessmentType
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
	FROM @tblTempTeacherSummary
	
END	

