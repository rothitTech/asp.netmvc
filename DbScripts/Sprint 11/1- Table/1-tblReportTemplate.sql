-- Author:  <Sachin Gupta>        
-- Create date: <12/15/2014>(mm/dd/yyyy)        
-- Description: <Create new table for Report Template>        
  
-- =============================================

IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblReportTemplate' )
BEGIN
	CREATE TABLE [dbo].[tblReportTemplate]
	(
		[TemplateId] [int] IDENTITY(1,1) NOT NULL,
		[TemplateName] [varchar](250) NOT NULL,
		CONSTRAINT Pk_tblReportTemplate PRIMARY KEY CLUSTERED([TemplateId]),
		CONSTRAINT [UQ_TemplateName] UNIQUE ([TemplateName])
	 );	
	
END
GO
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





