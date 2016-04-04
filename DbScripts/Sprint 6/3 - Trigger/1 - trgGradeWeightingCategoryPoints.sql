-- =============================================  
-- Author:  Sachin Gupta  
-- Create date: 10/09/14  
-- Description: <Description,,>  
-- =============================================  
CREATE TRIGGER [dbo].[trgGradeWeightingCategoryPoints]  
	ON  [dbo].[tblAssessmentGradeWeightingCategory]  
	AFTER INSERT, DELETE, UPDATE  
AS  
BEGIN  
	
	IF((SELECT TRIGGER_NESTLEVEL( OBJECT_ID('trgGradeWeightingCategoryPoints') , 'AFTER' , 'DML' ) )> 1) RETURN
	
	DECLARE @AssessmentGradeWeightingId int  
	DECLARE @TotalRcords DECIMAL(6,3)   
  
	If EXISTS(SELECT * FROM INSERTED)  
	BEGIN  
		SELECT @AssessmentGradeWeightingId = i.AssessmentGradeWeightingId FROM INSERTED i;   
	END   
	ELSE 
	BEGIN  
		SELECT @AssessmentGradeWeightingId = d.AssessmentGradeWeightingId FROM DELETED d;   
	END  

    SELECT @TotalRcords = (CASE WHEN COUNT(*) > 1 THEN COUNT(*) ELSE 2 END)  
	FROM  tblAssessmentGradeWeightingCategory   
	WHERE AssessmentGradeWeightingId = @AssessmentGradeWeightingId  
                  
  
	UPDATE [tblAssessmentGradeWeightingCategory]    
	SET Points = agw.Points  
	FROM   
	(	SELECT   
			AssessmentGradeWeightingCategoryid  
			,ROUND(CAST((ROW_NUMBER()OVER(PARTITION BY AssessmentGradeWeightingId ORDER BY MIN ASC) - 1) / (@TotalRcords-1)  AS DECIMAL(6,3)),2) AS Points  
		FROM [tblAssessmentGradeWeightingCategory]   
		WHERE [AssessmentGradeWeightingId]= @AssessmentGradeWeightingId
	) agw  
	WHERE [tblAssessmentGradeWeightingCategory].AssessmentGradeWeightingCategoryid = agw.AssessmentGradeWeightingCategoryid  
END	