

IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblAssessmentExpectation' )
BEGIN
	CREATE TABLE tblAssessmentExpectation
	(
		AssessmentExpectationId INT IDENTITY(1,1) NOT NULL,
		AssessmentId INT NOT NULL,
		Expectation Decimal(6,3) NOT NULL,
		CreateDatetime DATETIME NOT NULL DEFAULT(GetDate()),
                GradeLevel SMALLINT NOT NULL
		CONSTRAINT Pk_AssessmentExpectationId PRIMARY KEY (AssessmentExpectationId)
	); 

	ALTER TABLE tblAssessmentExpectation ADD CONSTRAINT Fk_AssessmentId FOREIGN KEY  (AssessmentId) REFERENCES tblAssessment(AssessmentId)
END
