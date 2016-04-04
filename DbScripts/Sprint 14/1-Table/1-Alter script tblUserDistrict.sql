----Drop unique constraint--------
ALTER TABLE dbo.tblUserDistrict DROP CONSTRAINT UNQ_UserDistrict_UserId_SchoolYearID


----Alter table columns-------
ALTER TABLE dbo.tblUserDistrict  
ALTER COLUMN SchoolYearId INT NOT NULL
GO

ALTER TABLE dbo.tblUserDistrict  
ALTER COLUMN RoleId INT NOT NULL 
GO
ALTER TABLE dbo.tblUserDistrict  
ALTER COLUMN ChangedDatetime DATETIME NULL
GO


-----------Add default value constriant------------------

ALTER TABLE  dbo.tblUserDistrict
ADD CONSTRAINT DF_tblUserDistrict_CreatedDatetime DEFAULT(GETDATE()) FOR [CreatedDatetime]


--------Add unique constraint------------------------------

ALTER TABLE dbo.tblUserDistrict
ADD CONSTRAINT UNQ_UserDistrict_UserId_SchoolYearID UNIQUE (DistrictId, UserId, SchoolYearId)


-------------Add Foreign key constraint---------------------------

ALTER TABLE dbo.tblUserDistrict ADD CONSTRAINT [FK_tblUserDistrict_tblRole] 
FOREIGN KEY(RoleId) REFERENCES  dbo.tblRole (RoleId)
GO

ALTER TABLE dbo.tblUserDistrict ADD CONSTRAINT [FK_tblUserDistrict_tblSchoolYear] 
FOREIGN KEY(SchoolYearId) REFERENCES  dbo.tblSchoolYear (SchoolYearId)
GO




