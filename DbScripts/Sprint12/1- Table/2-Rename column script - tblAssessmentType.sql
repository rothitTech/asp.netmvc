EXEC sp_rename 'tblAssessmentType.AssessmentTypeDesc', 'AssessmentCode', 'COLUMN';
EXEC sp_rename 'tblAssessmentType.AssessmentTypeFullDescText', 'AssessmentTypeDesc', 'COLUMN';