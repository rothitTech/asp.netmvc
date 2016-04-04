
IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'tblAssessmentMeetExceedCriteria' )
BEGIN
	CREATE TABLE tblAssessmentMeetExceedCriteria
	(
		AssessmentMeetExceedCriteriaId INT IDENTITY(1,1) NOT NULL,
		AssessmentWeightingId INT NOT NULL,
		StartRange INT NOT NULL,
		EndRange INT NOT NULL,
		CreateDatetime DATETIME NOT NULL DEFAULT(GetDate())          
		CONSTRAINT Pk_AssessmentMeetExceedCriteriaId PRIMARY KEY (AssessmentMeetExceedCriteriaId)
	); 
	ALTER TABLE tblAssessmentMeetExceedCriteria ADD CONSTRAINT AssessmentWeightingId FOREIGN KEY  (AssessmentWeightingId) REFERENCES tblAssessmentWeighting(AssessmentWeightingId)
END
