ALTER TABLE dbo.tblAssessmentType  
ADD ReportTemplateId INT NULL 
GO
UPDATE dbo.tblAssessmentType SET ReportTemplateId = 1 where AssessmentTypeDesc <>  'F & P'
UPDATE dbo.tblAssessmentType SET ReportTemplateId = 2 where AssessmentTypeDesc =  'F & P'
GO
ALTER TABLE dbo.tblAssessmentType  
ALTER COLUMN ReportTemplateId INT NOT NULL 
GO
INSERT INTO dbo.tblReportTemplate(TemplateName) values('MapTemplate')
INSERT INTO dbo.tblReportTemplate(TemplateName) values('FnPTemplate')
GO
ALTER TABLE dbo.tblAssessmentType 
ADD CONSTRAINT FK_tblAssessmentType_tblReportTemplate FOREIGN KEY  (ReportTemplateId)
REFERENCES dbo.tblReportTemplate(TemplateId)	


insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (1, 'ProjectionTitle', 'Projection')
insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (1, 'ShowToggleViewScaledScore', '1')
insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (1, 'DistrictPercentileTitle', 'Dist. %')

insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (2, 'ProjectionTitle', 'Expectation')
insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (2, 'ShowToggleViewScaledScore', '0')
insert into dbo.[tblReportTemplateConfiguration] (TemplateId, [Key], [Value]) values (2, 'DistrictPercentileTitle', 'Growth')

