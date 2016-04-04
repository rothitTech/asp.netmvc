USE [dbTIR]
GO
/****** Object:  StoredProcedure [dbo].[spGetTeacherAssessmentWithMEPerc]    Script Date: 1/13/2015 3:31:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spGetTeacherAssessmentWithMEPerc]   
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
	
	DECLARE @Query VARCHAR(MAX),
	        @tblTempStudents varchar(20)
	
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
	
	--Executing Teacher Impact Detail Assessment
	INSERT INTO @tblTempAssessmentWithMEPerc
	(AssessmentDesc,AssessmentId,TermId,OrderBy)
	EXEC [spGetTeacherImpactDetailAssessmentList] @SubjectId, @SchoolYearId, @AssessmentTypeId, @TeacherId, @GradeLevel, @ClassID
	
	-- Create and populate table used in spGetMeetOrExceedPerc
	SET @tblTempStudents ='tblTempStudents'
	CREATE TABLE #@tblTempStudents (StudentId int, CONSTRAINT PK_TempStudentId PRIMARY KEY CLUSTERED (StudentId))
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
		)

	--Exceuting Meet Exceed Percentage Proc
	INSERT INTO @tblTempMeetExceedPerc
	(SchoolTermId, MeetExceedPerc)
	EXEC [spGetMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
	@AssessmentTypeId, @TeacherId, @GradeLevel, @ClassId, @tblTempStudents

	DROP TABLE #@tblTempStudents
				
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