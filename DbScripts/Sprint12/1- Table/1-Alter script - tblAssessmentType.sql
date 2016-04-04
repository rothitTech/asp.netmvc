ALTER TABLE dbo.tblAssessmentType  
ADD AssessmentTypeFullDescText NVARCHAR(250) NULL 
GO
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'MAP' WHERE AssessmentTypeId = 1
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ISAT' WHERE AssessmentTypeId = 2
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'Fountas and Pinnell' WHERE AssessmentTypeId = 3
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'Heart Rate' WHERE AssessmentTypeId = 4
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT Composite' WHERE AssessmentTypeId = 5
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT English' WHERE AssessmentTypeId = 6
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT Math' WHERE AssessmentTypeId = 7
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT Reading' WHERE AssessmentTypeId = 8
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT Science' WHERE AssessmentTypeId = 9
UPDATE dbo.tblAssessmentType SET AssessmentTypeFullDescText = 'ACT' WHERE AssessmentTypeId = 10
GO
ALTER TABLE dbo.tblAssessmentType  
ALTER COLUMN AssessmentTypeFullDescText NVARCHAR(250) NOT NULL 
GO

DROP TABLE [tblReportTemplateConfiguration]

IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblReportTemplateConfiguration' )
BEGIN
	CREATE TABLE [dbo].[tblReportTemplateConfiguration]
	(
		[ConfigId] [int] IDENTITY(1,1) NOT NULL,
		[TemplateId] [int] NOT NULL,
		[Key] [varchar](250) NOT NULL,
		[Value] [varchar](250) NOT NULL,
		CONSTRAINT Pk_tblReportTemplateConfiguration PRIMARY KEY CLUSTERED([ConfigId]),
		CONSTRAINT [UQ_Key_TemplateId] UNIQUE ([TemplateId], [Key])
	 );	
	
END
GO
ALTER TABLE [dbo].[tblReportTemplateConfiguration]  ADD  CONSTRAINT [FK_tblReportTemplateConfiguration_tblReportTemplate] 
FOREIGN KEY([TemplateId]) REFERENCES  [dbo].[tblReportTemplate] ([TemplateId])
GO


INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (1, 'ProjectionTitle', 'Projection')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (1, 'ShowToggleViewScaledScore', '1')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (1, 'DistrictPercentileTitle', 'Dist. %')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (1, 'ShowScaledScoreByDefault', '0')

INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (2, 'ProjectionTitle', 'Expectation')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (2, 'ShowToggleViewScaledScore', '0')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (2, 'DistrictPercentileTitle', 'Growth')
INSERT INTO dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) VALUES (2, 'ShowScaledScoreByDefault', '1')

GO