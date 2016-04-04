IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblUserLoginInfo' )
BEGIN
	CREATE TABLE [dbo].[tblUserLoginInfo]
	(
		[LoginId] [int] IDENTITY(1,1) NOT NULL,
		[UserId] [int] NOT NULL,
		[LoginDate] [datetime] NULL
		CONSTRAINT Pk_LoginId PRIMARY KEY (LoginId)
	 );	
	ALTER TABLE tblUserLoginInfo ADD CONSTRAINT [FK_tblUser_tblUserLoginInfo]  FOREIGN KEY  (UserId) REFERENCES tblUser(UserId)	
END