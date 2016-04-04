-- =============================================  
-- Author:  Vikrant Arya  
-- Create date: 1310/2014      
-- Modified dt:   
-- Description: Proc to execute the Teacher Assessment Detail and Meet Exceed Percentage

-- Modified by : Hardev Gangwar 
-- Modified dt: 10/17/2014 
-- Description: return one more column MeetExceedCategory using function udfGetMeetExceedCategory
-- =============================================  
--EXEC [spGetTeacherAssessmentWithMEPerc] 1,1,3,1,17,5,28 

ALTER PROCEDURE [dbo].[spGetTeacherAssessmentWithMEPerc]   
	@DistrictId INT,
	@SubjectId INT,              
	@SchoolYearId INT,              
	@AssessmentTypeId INT,              
	@TeacherId INT,              
	@GradeLevel INT,          
	@ClassID  INT 
AS   
BEGIN
	
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
	
	--Exceuting Meet Exceed Percentage Proc
	INSERT INTO @tblTempMeetExceedPerc
	(SchoolTermId, MeetExceedPerc)
	EXEC [spGetMeetOrExceedPerc] @DistrictId, @SubjectId, @SchoolYearId, 
	@AssessmentTypeId, @TeacherId, @GradeLevel, @ClassId
			
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