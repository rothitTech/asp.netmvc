-- =============================================  
-- Author:  Vikrant Arya  
-- Create date: 09/10/2014      
-- Modified dt:   
-- Description: Proc to execute the Teacher Impact Summary and Meet Exceed Criteria
-- =============================================  
--EXEC [spGetTeacherSummaryView] 469,1,3,-1,1 

ALTER PROCEDURE [dbo].[spGetTeacherSummaryView]   
	@TeacherId INT,      
	@SchoolId INT,      
	@SchoolYearId INT,
	@ClassId INT, 
	@ViewMeetExceedSummary BIT
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
	)  
	
	DECLARE @tblTempMeetExceedPerc  TABLE
	(
		 SchoolTermId INT
		,MeetExceedPerc DECIMAL (6,3)
	)
	
	--Executing Teacher Impact Summary Proc
	INSERT INTO @tblTempTeacherSummary
	(Grade,AssessmentType,ClassSubject,AssessmentTypeId,Weighting,
	 Impact,SubjectId,NoOfStudent,Average,AssessmentGradeWeightingId,SchoolTermId)
	EXEC [spGetTeacherImpactSummary] @TeacherId, @SchoolId, @SchoolYearId, @ClassId
	
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
			WHERE CounterId = 	@Counter	
			
			DELETE FROM @tblTempMeetExceedPerc
			
			SET @Counter = @Counter + 1
		END
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
	FROM @tblTempTeacherSummary
	
END	