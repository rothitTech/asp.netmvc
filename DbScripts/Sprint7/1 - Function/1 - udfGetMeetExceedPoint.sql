USE [dbTIR]
GO
/****** Object:  UserDefinedFunction [dbo].[udfGetMeetExceedPoint]    Script Date: 13/10/2014 10:53:10 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Hardev Gangwar
-- Create date: 10/08/14
-- Description:	udfGetMeetExceedPoint function return meet/Exceed points value
-- =============================================

-- select dbo.udfGetMeetExceedPoint(1,1,3,1,3,90)
ALTER FUNCTION [dbo].[udfGetMeetExceedPoint]
(
   @DistrictId INT,
   @SubjectId INT,            
   @SchoolYearId INT,            
   @AssessmentTypeId INT,  
   @Grade INT,
   @MeetExceedPerc DECIMAL (6,3)      
)
RETURNS DECIMAL (6,3)	
AS
BEGIN	
	DECLARE @MeetExceedPoint DECIMAL (6,3)	 
	IF @MeetExceedPerc IS NOT NULL
	BEGIN
		SELECT  @MeetExceedPoint = agwc.Points								
		FROM tblAssessmentGradeWeightingCategory agwc 
		JOIN tblAssessmentGradeWeighting agw ON agwc.AssessmentGradeWeightingId = agw.AssessmentGradeWeightingId
		JOIN tblAssessmentWeighting aw ON  agw.AssessmentWeightingId = aw.AssessmentWeightingId
		JOIN tblWeightingCategory wc on agwc.CategoryId = wc.CategoryId
		WHERE agw.Grade = @Grade AND aw.DistrictId = @DistrictId AND aw.AssessmentTypeId = @AssessmentTypeId 
		AND aw.SchoolYearId = @SchoolYearId AND aw.SubjectId = @SubjectId 
		AND @MeetExceedPerc >= agwc.[Min] AND @MeetExceedPerc <= agwc.[Max]
	END	
	RETURN @MeetExceedPoint
END
