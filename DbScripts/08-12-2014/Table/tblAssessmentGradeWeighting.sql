IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblAssessmentGradeWeighting' )
BEGIN
	CREATE TABLE tblAssessmentGradeWeighting
	(
		AssessmentGradeWeightingId INT IDENTITY(1,1) NOT NULL,
		AssessmentWeightingId INT NOT NULL,
		Grade SMALLINT NOT NULL,
		Weighting DECIMAL(5,2) NOT NULL
		CONSTRAINT Pk_AssessmentGradeWeightingId PRIMARY KEY (AssessmentGradeWeightingId)
	); 

	ALTER TABLE tblAssessmentGradeWeighting ADD CONSTRAINT Fk_AssessmentWeightingID 
	FOREIGN KEY  (AssessmentWeightingId) REFERENCES tblAssessmentWeighting(AssessmentWeightingId)
END


