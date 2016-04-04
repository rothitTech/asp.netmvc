ALTER TABLE tblStudentSchoolYear 
ADD CONSTRAINT AK_StudentSchoolYear_StudentId_SchoolYearId UNIQUE (StudentId, SchoolYearId)
	
