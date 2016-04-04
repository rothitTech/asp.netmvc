ALTER TABLE dbo.tblAssessmentGradeWeightingCategory 
ADD Points decimal(6,3)  NULL


ALTER TABLE tblAssessmentGradeWeightingCategory 
ADD CONSTRAINT AK_tblAssessmentGradeWeightingCategory_AssessmentGradeWeightingId_CategoryId UNIQUE (AssessmentGradeWeightingId, CategoryId)
