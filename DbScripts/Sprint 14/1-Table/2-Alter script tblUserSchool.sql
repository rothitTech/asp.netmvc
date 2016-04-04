----Drop unique constraint--------
ALTER TABLE dbo.tblUserSchool DROP CONSTRAINT UNQ_UserSchool_UserId_SchoolYearID


----Alter table columns-------

ALTER TABLE dbo.tblUserSchool  
ALTER COLUMN SchoolYearId INT NOT NULL 
GO

ALTER TABLE dbo.tblUserSchool  
ALTER COLUMN ChangedDatetime DATETIME NULL 
GO

-----------Add default value constriant------------------

ALTER TABLE  dbo.tblUserSchool
ADD CONSTRAINT DF_tblUserSchool_CreatedDatetime DEFAULT(GETDATE()) FOR [CreatedDatetime] 


--------Add unique constraint------------------------------

ALTER TABLE dbo.tblUserSchool
ADD CONSTRAINT UNQ_UserSchool_UserId_SchoolYearID UNIQUE (SchoolId, UserId, SchoolYearId)

-------------Add Foreign key constraint---------------------------

ALTER TABLE dbo.tblUserSchool ADD CONSTRAINT [FK_tblUserSchool_tblSchoolYear] 
FOREIGN KEY(SchoolYearId) REFERENCES  dbo.tblSchoolYear (SchoolYearId)
GO
-------------------------------------------------------------------------------
