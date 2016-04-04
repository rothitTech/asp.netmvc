ALTER TABLE dbo.tblstudentschoolYear  
ADD Hispanic BIT NOT NULL 
CONSTRAINT DF_Hispanic_Value DEFAULT (0)

ALTER TABLE dbo.tblstudentschoolYear  
ALTER COLUMN LepIndicator BIT NOT NULL

ALTER TABLE dbo.tblstudentschoolYear  
ALTER COLUMN IepIndicator BIT NOT NULL

ALTER TABLE dbo.tblstudentschoolYear  
ALTER COLUMN FrlIndicator BIT NOT NULL

