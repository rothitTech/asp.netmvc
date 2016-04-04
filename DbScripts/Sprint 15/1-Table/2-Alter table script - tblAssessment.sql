ALTER TABLE dbo.tblAssessment
Add ParentAssessmentId INT NULL
GO

-------------Add Foreign key constraint----------------------

ALTER TABLE dbo.tblAssessment ADD CONSTRAINT [FK_ParentAssessmentId] 
FOREIGN KEY(ParentAssessmentId) REFERENCES  dbo.tblAssessment (AssessmentId)
GO