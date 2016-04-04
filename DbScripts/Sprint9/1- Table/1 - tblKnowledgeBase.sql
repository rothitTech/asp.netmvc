
-- Author:  <Hardev Gangwar>        
  
-- Create date: <11/20/2014>        
  
-- Description: <Create new table for Knowledge Base>        
  
-- =============================================

IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblKnowledgeBase' )
BEGIN
	CREATE TABLE [dbo].[tblKnowledgeBase]
	(
		[KnowledgeBaseId] [int] IDENTITY(1,1) NOT NULL,
		[Title] [varchar](250) NOT NULL,
		[Text] [varchar](max) NOT NULL,
		[DistrictId] [int] NULL,
		[RoleId] [int] NOT NULL,
		[CreateDatetime] [datetime] NOT NULL DEFAULT(GetDate())
		CONSTRAINT Pk_KnowledgeBaseId PRIMARY KEY (KnowledgeBaseId)
	 );	
	ALTER TABLE tblKnowledgeBase ADD CONSTRAINT DistrictId FOREIGN KEY  (DistrictId) REFERENCES tblDistrict(DistrictId)	
	ALTER TABLE tblKnowledgeBase ADD CONSTRAINT RoleId FOREIGN KEY  (RoleId) REFERENCES tblRole(RoleId)	
END