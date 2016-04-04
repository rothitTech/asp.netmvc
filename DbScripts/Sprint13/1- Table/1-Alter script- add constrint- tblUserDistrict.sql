ALTER TABLE dbo.tblUserDistrict DROP CONSTRAINT UNQ_UserDistrict_UserId

ALTER TABLE dbo.tblUserDistrict
ADD CONSTRAINT UNQ_UserDistrict_UserId_SchoolYearID UNIQUE (DistrictId, UserId, SchoolYearId)