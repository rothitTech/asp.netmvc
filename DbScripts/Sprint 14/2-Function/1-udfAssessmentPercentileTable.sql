USE [dbTIR]
GO
/****** Object:  UserDefinedFunction [dbo].[udfAssessmentPercentileTable]    Script Date: 1/23/2015 2:12:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Henry Schwenk
-- Create date: 2/13/14
-- Description:	Returns an Assessment % table

-- Modified by: Sachin Gupta
-- Modified date : 29/08/2014
-- Changes done as per US- 93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff 
-- should also be null )

-- Modified by: Henry Schwenk
-- Modified date: 22/1/2015
-- Added "Score is not null" to to ppart of CTE to prevent null scores from being included in dist% calculation
-- =============================================

ALTER FUNCTION [dbo].[udfAssessmentPercentileTable] 
(
	@AssessmentId int,
	@DistrictId int,
	@GradeLevel int
)

RETURNS @AssessmentPercentile TABLE 
(
	AssessmentScoreId int NOT NULL, 
	StudentId int NOT NULL, 
	Score int NULL, -- converted to nullable column
	Percentile decimal(4,1) NOT NULL
)

AS

BEGIN
	WITH ScoresRnkCnt AS
	(
		select 
			AssessmentScoreId, 
			StudentId, 
			Score,
			RANK() OVER(ORDER BY Score) AS rnk,
			COUNT(*) Over(Partition by Score) as cnt,
			COUNT(*) OVER() AS total
		from 
			tblAssessmentScore 
		where 
			AssessmentId = @AssessmentId
			and
			GradeLevel = @GradeLevel
			and
			SchoolId in (select SchoolId from tblSchool where DistrictID = @DistrictId)
			and
			Score is not null
	)

	insert @AssessmentPercentile
	select 
		AssessmentScoreId, 
		StudentId, 
		Score,
		--Cast(100.*(rnk-1)/(total-1) as decimal(4,1)) AS Percentile
		Cast(100.*((rnk-1)+(0.5*cnt))/(total) as decimal(4,1)) AS Percentile
	from 
		ScoresRnkCnt
	order by 
		Score;
	RETURN 

END