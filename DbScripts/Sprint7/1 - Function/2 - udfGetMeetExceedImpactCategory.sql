SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Vikrant Arya
-- Create date: 10/20/14
-- Description:	Function to retrieve the ribbon color value
-- =============================================
--SELECT dbo.[udfGetMeetExceedRibbonValue] (0.2529)


CREATE FUNCTION [dbo].[udfGetMeetExceedImpactCategory]
(
   @TotalImpactScore DECIMAL (6,3)      
)
RETURNS INT
AS
BEGIN
	
	DECLARE @MERibbonValue INT
	SET @MERibbonValue = -3

	
	BEGIN
		SELECT  
			@MERibbonValue =	CASE  
			                          WHEN (@TotalImpactScore >= .00) AND (@TotalImpactScore <= .24) THEN -2 
								      WHEN (@TotalImpactScore >= .25) AND (@TotalImpactScore <= .49) THEN -1 
								      WHEN (@TotalImpactScore >= .50) AND (@TotalImpactScore <= .74) THEN 0 
								      WHEN (@TotalImpactScore >= .75) AND (@TotalImpactScore <= 1.00) THEN 1 
							    END
	END

	RETURN ISNULL(@MERibbonValue, -3)
END
