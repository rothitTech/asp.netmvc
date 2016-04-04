IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblAnnouncement' )
BEGIN
	CREATE TABLE tblAnnouncement
	(
		AnnouncementId INT IDENTITY(1,1) NOT NULL,
		Title VARCHAR(250) NOT NULL,
		[Text] VARCHAR(MAX) NOT NULL,
		DistrictId INT NULL,
		RoleId INT NOT NULL,
		CreateDatetime DATETIME NOT NULL DEFAULT(GetDate())
		CONSTRAINT Pk_AnnouncementId PRIMARY KEY (AnnouncementId)
	); 

	ALTER TABLE tblAnnouncement ADD CONSTRAINT Fk_DistrictId FOREIGN KEY  (DistrictId) REFERENCES tblDistrict(DistrictId)
	
	ALTER TABLE tblAnnouncement ADD CONSTRAINT Fk_RoleId FOREIGN KEY  (RoleId) REFERENCES tblRole(RoleId)
END


