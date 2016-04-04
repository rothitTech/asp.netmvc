IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'tblAssessmentTemplate' )
BEGIN
	CREATE TABLE [dbo].[tblAssessmentTemplate]
	(
		[AssessmentTemplateId] [int] IDENTITY(1,1) NOT NULL,
		[TemplateType] [varchar](250) NULL,
		[TemplateFileName] [varchar](250) NULL,
		CONSTRAINT Pk_tblAssessmentTemplate PRIMARY KEY CLUSTERED([AssessmentTemplateId])
	 );	
	
END
GO


INSERT INTO tblAssessmentTemplate VALUES('SLO', 'slo-4th-grade-math.pdf')
INSERT INTO tblAssessmentTemplate VALUES('Rubric', 'PerformanceTaskRubric.pdf')




