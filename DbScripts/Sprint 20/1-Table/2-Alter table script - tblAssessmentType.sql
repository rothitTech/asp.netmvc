ALTER TABLE dbo.tblAssessmentType
Add ParentAssessmentTypeId INT NULL
GO

-------------Add Foreign key constraint----------------------

ALTER TABLE dbo.tblAssessmentType ADD CONSTRAINT [FK_ParentAssessmentTypeId] 
FOREIGN KEY(ParentAssessmentTypeId) REFERENCES  dbo.tblAssessmentType (AssessmentTypeId)
GO