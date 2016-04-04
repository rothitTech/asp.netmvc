-- =============================================


-- Author:		Henry Schwenk
-- Create date: 2/13/14
-- Description:	Rescale an Assessment Score


-- Edited by: Sachin Gupta
-- Modified date : 29/08/2014
-- Changes done as per US- 93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff 
 -- should also be null )



-- =============================================



ALTER FUNCTION [dbo].[udfRescaleAssessmentScore] 
(
	@AssessmentScoreId int
)

RETURNS decimal(6,3)

AS

BEGIN

	DECLARE 


		@ScaledScore decimal(6,3),

		@AssessmentId int,

		@AssessmentScore decimal(6,3),

		@AssessmentScoreMin decimal(6,3),

		@AssessmentScoreMax decimal(6,3)


		select 

			@AssessmentId = AssessmentId, 

			@AssessmentScore = cast(Score as decimal(6,3))

		from 

			[dbo].[tblAssessmentScore]

		where 

			[AssessmentScoreId] = @AssessmentScoreId


		select 

			@AssessmentScoreMin = cast(ScoreMin as decimal(6,3)),

			@AssessmentScoreMax = cast(ScoreMax as decimal(6,3))

		from 

			[dbo].[tblAssessment]

		where

			[AssessmentId] = @AssessmentId

		--  check added if @AssessmentScore has null value then to skip calcualtion


		if @AssessmentScore is  NULL
		begin
		  SET @ScaledScore = NULL
		end
		else
		begin
		   if isnull(@AssessmentScoreMax, 0) - isnull(@AssessmentScoreMin, 0) <> 0
			set @ScaledScore = 100 * ((@AssessmentScore - @AssessmentScoreMin) / (@AssessmentScoreMax - @AssessmentScoreMin))
		end


	RETURN @ScaledScore


END