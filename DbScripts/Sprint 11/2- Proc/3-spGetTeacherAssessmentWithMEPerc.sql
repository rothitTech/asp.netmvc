IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetTeacherAssessmentWithMEPerc')
BEGIN
	DROP PROCEDURE spGetTeacherAssessmentWithMEPerc
END	
GO
CREATE PROCEDURE [dbo].[spGetTeacherAssessmentWithMEPerc]   
	@DistrictId INT,
	@SubjectId INT,              
	@SchoolYearId INT,              
	@AssessmentTypeId INT,              
	@TeacherId INT,              
	@GradeLevel INT,          
	@ClassID  INT,
	@Race INT = NULL,
	@Gender INT = NULL,
	@FrlIndicator BIT = NULL,
	@IepIndicator BIT = NULL,
	@LepIndicator BIT = NULL,
	@Hispanic BIT = NULL 
AS   
BEGIN
	
	DECLARE @Query VARCHAR(MAX)
	
	DECLARE @tblTempAssessmentWithMEPerc  TABLE
	(            
		 CounterId INT IDENTITY(1,1)        
		,AssessmentDesc NVARCHAR(256)
		,AssessmentId INT
		,TermId INT
		,OrderBy INT
		,MeetExceedPerc DECIMAL (6,3)
		,MeetExceedCategory INT 
	)  
	
	DECLARE @tblTempMeetExceedPerc  TABLE
	(
		 SchoolTermId INT
		,MeetExceedPerc DECIMAL (6,3)
	)
	
	DECLARE @tblTempStudents as StudentType
	INSERT INTO @tblTempStudents SELECT S.STUDENTID FROM tblStudent S 
      INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
      WHERE SSY.SchoolYearId =@schoolYearId
      AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
      AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
      AND (@Hispanic IS NULL OR SSY.Hispanic = @Hispanic)
      AND (@Race IS NULL OR S.RaceId = @Race)
      AND (@Gender IS NULL OR S.GenderId = @Gender)	
	
	--Executing Teacher Impact Detail Assessment
	INSERT INTO @tblTempAssessmentWithMEPerc
	(AssessmentDesc,AssessmentId,TermId,OrderBy)
	EXEC [spGetTeacherImpactDetailAssessmentList] @SubjectId, @SchoolYearId, @AssessmentTypeId, @TeacherId, @GradeLevel, @ClassID
	
	--Exceuting Meet Exceed Percentage Proc
	INSERT INTO @tblTempMeetExceedPerc
	(SchoolTermId, MeetExceedPerc)
	EXEC [spGetMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
	@AssessmentTypeId, @TeacherId, @GradeLevel, @ClassId,@tblTempStudents
				
	--Updating the Meet Exceed Precentage and Meet Exceed Category			
	UPDATE @tblTempAssessmentWithMEPerc
	SET MeetExceedPerc = MEP.MeetExceedPerc
	,MeetExceedCategory = (SELECT [dbo].[udfGetMeetExceedCategory] (@DistrictId, @SubjectId, @SchoolYearId, @AssessmentTypeId, @GradeLevel, MEP.MeetExceedPerc ))         
	FROM @tblTempMeetExceedPerc MEP
	WHERE TermId = MEP.SchoolTermId	
	
	SELECT  
		 AssessmentDesc
		,AssessmentId
		,TermId
		,OrderBy
		,MeetExceedPerc
		,MeetExceedCategory  
	FROM @tblTempAssessmentWithMEPerc
	
END