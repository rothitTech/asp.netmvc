-- =============================================  
-- Author:  Vikrant Arya  
-- Create date: 1310/2014      
-- Modified dt:   
-- Description: Proc to execute the Teacher Assessment Detail and Meet Exceed Percentage

-- Modified by : Hardev Gangwar 
-- Modified dt: 10/17/2014 
-- Description: return one more column MeetExceedCategory using function udfGetMeetExceedCategory

-- Modified by: Vikrant Arya                   
-- Modified date : 11/21/2014                    
-- Description : Filter student against the selected report filters
-- =============================================  
--EXEC [spGetTeacherAssessmentWithMEPerc] 1,1,3,1,17,5,28 

--Drop Proc if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='P' AND name = 'spGetTeacherAssessmentWithMEPerc')
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
	
	--Executing Teacher Impact Detail Assessment
	INSERT INTO @tblTempAssessmentWithMEPerc
	(AssessmentDesc,AssessmentId,TermId,OrderBy)
	EXEC [spGetTeacherImpactDetailAssessmentList] @SubjectId, @SchoolYearId, @AssessmentTypeId, @TeacherId, @GradeLevel, @ClassID
	
	--Exceuting Meet Exceed Percentage Proc
	INSERT INTO @tblTempMeetExceedPerc
	(SchoolTermId, MeetExceedPerc)
	EXEC [spGetMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
	@AssessmentTypeId, @TeacherId, @GradeLevel, @ClassId
	
	IF EXISTS(SELECT [name] FROM tempdb.sys.tables WHERE [name] like '#tblTempStudents%')
	BEGIN
		DROP TABLE #tblTempStudents
	END
			
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