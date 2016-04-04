USE [dbTIR]
GO
/****** Object:  StoredProcedure [dbo].[spGetHeirarchicalReportMEPercentage]    Script Date: 3/17/2015 6:28:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- exec spGetHeirarchicalReportMEPercentage 1,7,3,11,463,3,-1,1

ALTER PROCEDURE [dbo].[spGetHeirarchicalReportMEPercentage]   
	@DistrictId INT,
	@SubjectId INT,              
	@SchoolYearId INT,              
	@AssessmentTypeId INT,              
	@TeacherId INT,              
	@GradeLevel INT,          
	@ClassID  INT,
	@SchoolTermId INT,
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
		TermId INT      
		,MeetExceedPerc DECIMAL (6,3)
		,MeetExceedCategory INT 
	)  
	
	DECLARE @tblTempMeetExceedPerc  TABLE
	(
		MeetExceedPerc DECIMAL (6,3)
	)
	

	INSERT INTO @tblTempAssessmentWithMEPerc
	(TermId) VALUES(@SchoolTermId)


	-- Create and populate table used in spGetMeetOrExceedPerc
	SET @tblTempStudents ='tblTempStudents'
	CREATE TABLE #@tblTempStudents (StudentId int, CONSTRAINT PK_TempStudentId PRIMARY KEY CLUSTERED (StudentId))
	INSERT INTO #@tblTempStudents SELECT S.STUDENTID FROM tblStudent S 
      INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
      WHERE SSY.SchoolYearId = @schoolYearId
	  AND SSY.GradeLevel = @GradeLevel
	  AND SSY.ServingSchoolId in (select schoolId from tblSchool where DistrictId = @DistrictId)
      AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
      AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
      AND (@FrlIndicator IS NULL OR SSY.FrlIndicator = @FrlIndicator)
      AND (@Hispanic IS NULL OR SSY.Hispanic = @Hispanic)
      AND (@Race IS NULL OR S.RaceId = @Race)
      AND (@Gender IS NULL OR S.GenderId = @Gender)
	  AND S.STUDENTID IN
	  (
		SELECT DISTINCT cs.StudentId
		FROM tblClass c
			join tblClassTeacher ct ON ct.ClassId = c.classId
			join tblClassStudent cs ON cs.ClassId = c.classId
		WHERE c.SchoolYearId = @SchoolYearId
		and ct.UserId = @TeacherId
		)

	--Exceuting Meet Exceed Percentage Proc
	INSERT INTO @tblTempMeetExceedPerc
	(MeetExceedPerc)
	EXEC [spGetHeirarchicalReportMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
	@AssessmentTypeId, @TeacherId, @GradeLevel, @ClassId, @SchoolTermId, @tblTempStudents

	DROP TABLE #@tblTempStudents
				
	--Updating the Meet Exceed Precentage and Meet Exceed Category			
	UPDATE @tblTempAssessmentWithMEPerc
	SET MeetExceedPerc = MEP.MeetExceedPerc
	,MeetExceedCategory = (SELECT [dbo].[udfGetMeetExceedCategory] (@DistrictId, @SubjectId, @SchoolYearId, @AssessmentTypeId, @GradeLevel, MEP.MeetExceedPerc ))         
	FROM @tblTempMeetExceedPerc MEP
	WHERE TermId = @SchoolTermId	
	
	SELECT  
		 MeetExceedPerc
		,MeetExceedCategory  
	FROM @tblTempAssessmentWithMEPerc
	
END