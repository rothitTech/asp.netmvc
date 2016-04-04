BEGIN

	CREATE TABLE tblWeightingCategory
	(
		CategoryId INT  NOT NULL,
		CategoryDesc VARCHAR(250) NOT NULL
		CONSTRAINT Pk_WeightingCategoryId PRIMARY KEY (CategoryId)
	); 

	CREATE TABLE tblAssessmentGradeWeightingCategory
	(
		AssessmentGradeWeightingCategoryId INT IDENTITY(1,1) NOT NULL,
		AssessmentGradeWeightingId INT,
		[Min] DECIMAL(5,2) NOT NULL,
		[Max] DECIMAL(5,2) NOT NULL,
		CategoryId INT  NOT NULL
		CONSTRAINT Pk_AssessmentGradeWeightingCategoryId PRIMARY KEY (AssessmentGradeWeightingCategoryId)
	); 

	ALTER TABLE tblAssessmentGradeWeightingCategory ADD CONSTRAINT Fk_AssessmentGradeWeightingId 
	FOREIGN KEY  (AssessmentGradeWeightingId) REFERENCES tblAssessmentGradeWeighting(AssessmentGradeWeightingId)
	
	ALTER TABLE tblAssessmentGradeWeightingCategory ADD CONSTRAINT Fk_CategoryId 
	FOREIGN KEY  (CategoryId) REFERENCES tblWeightingCategory(CategoryId)
END


