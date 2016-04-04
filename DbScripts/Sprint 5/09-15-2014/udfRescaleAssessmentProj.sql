-- =============================================  
-- Author:  Henry Schwenk  
-- Create date: 3/14/14  
-- Description: Rescale an Assessment Projection  
-- =============================================  
ALTER FUNCTION [dbo].[udfRescaleAssessmentProj]  
(  
 @AssessmentScoreId int  
)  
  
RETURNS decimal(6,3)  
AS  
BEGIN  
 DECLARE   
  @ScaledProj decimal(6,3),  
  @AssessmentId int,  
  @AssessmentProj decimal(6,3),  
  @AssessmentScoreMin decimal(6,3),  
  @AssessmentScoreMax decimal(6,3)  
  
  select   
   @AssessmentId = AssessmentId,   
   @AssessmentProj = cast(Projection as decimal(6,3))  
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
  
  if isnull(@AssessmentScoreMax, 0) - isnull(@AssessmentScoreMin, 0) <> 0  
   set @ScaledProj = ROUND(100 * ((@AssessmentProj - @AssessmentScoreMin) / (@AssessmentScoreMax - @AssessmentScoreMin)), 1 , 1) 
  
 RETURN @ScaledProj  
  
END