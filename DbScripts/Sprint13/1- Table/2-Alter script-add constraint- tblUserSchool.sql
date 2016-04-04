ALTER TABLE dbo.tblUserSchool
ADD CONSTRAINT UNQ_UserSchool_UserId_SchoolYearID UNIQUE (SchoolId, UserId, SchoolYearId)