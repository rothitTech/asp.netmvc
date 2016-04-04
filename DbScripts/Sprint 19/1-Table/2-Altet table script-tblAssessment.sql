ALTER TABLE dbo.tblAssessment  
ADD SLOTemplateId INT NULL, RubricTemplateId INT NULL 
GO

-- Replace AssessmentId and run this query

--UPDATE tblAssessment SET SLOTemplateId= 1, RubricTemplateId= 2 
--WHERE AssessmentId = 345 


ALTER TABLE [dbo].[tblAssessment]  ADD  CONSTRAINT [FK_tblAssessment-SLOTemplateId_tblAssessmentTemplate] 
FOREIGN KEY([SLOTemplateId]) REFERENCES [dbo].[tblAssessmentTemplate] ([AssessmentTemplateId])
GO

ALTER TABLE [dbo].[tblAssessment]  ADD  CONSTRAINT [FK_tblAssessment-RubricTemplateId_tblAssessmentTemplate] 
FOREIGN KEY([RubricTemplateId]) REFERENCES  [dbo].[tblAssessmentTemplate] ([AssessmentTemplateId])
GO




