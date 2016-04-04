ALTER TABLE [tblAssessmentScore]
   DROP COLUMN ScoreProjDif

ALTER TABLE [tblAssessmentScore] ALTER COLUMN Score INT NULL

ALTER TABLE [tblAssessmentScore]
   ADD ScoreProjDif AS ( ([Score]-[Projection])) Persisted
