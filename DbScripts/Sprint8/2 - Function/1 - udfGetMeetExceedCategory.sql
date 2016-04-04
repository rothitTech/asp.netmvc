USE [dbTIR]
GO
/****** Object:  UserDefinedFunction [dbo].[udfGetMeetExceedCategory]    Script Date: 10/10/2014 10:53:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sachin Gupta
-- Create date: 10/08/14
-- Description:	udfGetMeetExceedCategory returns Meet Exceed Category value on basis of MeetExceedPerc
-- =============================================

-- Modified by: Sachin Gupta
-- Modified date: 10/30/14
-- Description:	Modfied to handle null value
-- =============================================
-- select dbo.udfGetMeetExceedCategory(1,1,3,1,3,99)

--Drop function if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='FN' AND name = 'udfGetMeetExceedCategory')
BEGIN
	DROP Function udfGetMeetExceedCategory
END	
GO
CREATE FUNCTION [dbo].[udfGetMeetExceedCategory]
(
   @DistrictId INT,
   @SubjectId INT,            
   @SchoolYearId INT,            
   @AssessmentTypeId INT,  
   @Grade INT,
   @MeetExceedPerc DECIMAL (6,3)      
)
RETURNS INT
AS
BEGIN
	
	DECLARE @MeetExceedCategory INT,
	@CategoryDesc VARCHAR(250)
	 
	IF @MeetExceedPerc IS NOT NULL
	BEGIN
		SELECT  @MeetExceedCategory = CASE WHEN (CategoryDesc = 'Exceeds') THEN 1 
											WHEN (CategoryDesc = 'Meets') THEN 0
											WHEN (CategoryDesc = 'Below') THEN -1
											WHEN (CategoryDesc = 'Unsatisfactory') THEN -2
										END									
		FROM tblAssessmentGradeWeightingCategory agwc 
		JOIN tblAssessmentGradeWeighting agw ON agwc.AssessmentGradeWeightingId = agw.AssessmentGradeWeightingId
		JOIN tblAssessmentWeighting aw ON  agw.AssessmentWeightingId = aw.AssessmentWeightingId
		JOIN tblWeightingCategory wc on agwc.CategoryId = wc.CategoryId
		WHERE agw.Grade = @Grade AND aw.DistrictId = @DistrictId AND aw.AssessmentTypeId = @AssessmentTypeId 
		AND aw.SchoolYearId = @SchoolYearId AND aw.SubjectId = @SubjectId 
		AND @MeetExceedPerc >= agwc.[Min] AND @MeetExceedPerc <= agwc.[Max]
	END	
	RETURN @MeetExceedCategory
END
