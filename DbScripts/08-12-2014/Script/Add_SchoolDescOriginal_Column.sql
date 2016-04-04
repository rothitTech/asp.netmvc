USE [dbTIR]

ALTER TABLE [dbo].[tblSchool] ADD [SchoolDescOriginal] NVARCHAR(50) NULL 
GO

update tblSchool set SchoolDescOriginal = 'Braeside Elementary School' where SchoolId = 1
update tblSchool set SchoolDescOriginal = 'Indian Trail Elementary School' where SchoolId = 2
update tblSchool set SchoolDescOriginal = 'Lincoln Elementary School' where SchoolId = 3
update tblSchool set SchoolDescOriginal = 'Oak Terrace Elementary School' where SchoolId = 4
update tblSchool set SchoolDescOriginal = 'Ravinia Elementary School' where SchoolId = 5
update tblSchool set SchoolDescOriginal = 'Red Oak Elementary School' where SchoolId = 6
update tblSchool set SchoolDescOriginal = 'Sherwood Elementary School' where SchoolId = 7
update tblSchool set SchoolDescOriginal = 'Wayne Thomas Elementary School' where SchoolId = 8
update tblSchool set SchoolDescOriginal = 'Edgewood Middle School' where SchoolId = 9
update tblSchool set SchoolDescOriginal = 'Elm Place Middle School' where SchoolId = 10
update tblSchool set SchoolDescOriginal = 'Northwood Junior High School' where SchoolId = 11
update tblSchool set SchoolDescOriginal = 'Highland Park High School' where SchoolId = 12
update tblSchool set SchoolDescOriginal = 'Deerfield High School' where SchoolId = 13
update tblSchool set SchoolDescOriginal = 'Kipling Elementary School' where SchoolId = 14
update tblSchool set SchoolDescOriginal = 'South Park Elementary School' where SchoolId = 15
update tblSchool set SchoolDescOriginal = 'Walden Elementary School' where SchoolId = 16
update tblSchool set SchoolDescOriginal = 'Wilmot Elementary School' where SchoolId = 17
update tblSchool set SchoolDescOriginal = 'Alan B. Shepard Middle School' where SchoolId = 18
update tblSchool set SchoolDescOriginal = 'Charles J. Caruso Middle School' where SchoolId = 19
update tblSchool set SchoolDescOriginal = 'Green Bay School' where SchoolId = 20