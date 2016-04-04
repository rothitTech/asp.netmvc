BEGIN TRAN
	Update tblAssessmentScore Set score = score + 1 from tblAssessmentScore 
	IF (@@ERROR <> 0) GOTO ERR_HANDLER

	Update tblAssessmentScore Set score = score - 1 from tblAssessmentScore 
	IF (@@ERROR <> 0) GOTO ERR_HANDLER

	Update tblAssessmentScore Set projection = projection + 1 from tblAssessmentScore 
	IF (@@ERROR <> 0) GOTO ERR_HANDLER

	Update tblAssessmentScore Set projection = projection - 1 from tblAssessmentScore 
	IF (@@ERROR <> 0) GOTO ERR_HANDLER
COMMIT TRAN	

ERR_HANDLER:
	IF (@@ERROR <> 0) ROLLBACK TRAN
