IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetTeacherImpactDetail')
BEGIN
	DROP PROCEDURE spGetTeacherImpactDetail
END	
GO
CREATE PROCEDURE [dbo].[spGetTeacherImpactDetail]     
	@SubjectId INT,    
    @SchoolYearId INT,    
    @AssessmentTypeId INT,    
    @TeacherId INT,    
    @GradeLevel INT,         
    @ViewScaledScore BIT,    
    @ClassID  INT ,    
    @DistrictId INT,
    @Race INT = NULL,
	@Gender INT = NULL,
	@FrlIndicator BIT = NULL,
	@IepIndicator BIT = NULL,
	@LepIndicator BIT = NULL,
	@Hispanic BIT = NULL   
AS     
    
	DECLARE @AssessmentType  VARCHAR(20)      
			,@Query VARCHAR(MAX)
    SELECT  @AssessmentType = AssessmentTypeDesc     
    FROM tblassessmenttype     
    WHERE AssessmentTypeId= @AssessmentTypeId      
    
    -- Use Table Variable for class filter                
	DECLARE @tblTempClassID TABLE    
    (              
		ID INT IDENTITY(1,1),          
    	ClassID int    
    )    
    
	IF @ClassID = -1                
    BEGIN    
		INSERT INTO @tblTempClassID(ClassID)    
		SELECT c.ClassId       
		FROM tblClass c       
		JOIN tblClassTeacher ct ON c.ClassId = ct.ClassId       
		JOIN tblUser u ON ct.UserId = u.UserId       
		WHERE c.SchoolYearId = @SchoolYearId       
		--AND c.SubjectId = @SubjectId        
		AND u.UserId = @TeacherId    
	END    
    ELSE    
    BEGIN     
		INSERT INTO @tblTempClassID(ClassID)                  
		SELECT @ClassID                
	END                  
    

    DECLARE @tblTempStudents TABLE
	(
		studentId int not null
	)
          
	INSERT INTO @tblTempStudents SELECT S.STUDENTID FROM tblStudent S 
	INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
	WHERE SSY.SchoolYearId =@schoolYearId
	AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
	AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
	AND (@Hispanic IS NULL OR SSY.Hispanic = @Hispanic)
	AND (@Race IS NULL OR S.RaceId = @Race)
	AND (@Gender IS NULL OR S.GenderId = @Gender)
    
    BEGIN    
		SELECT 
			s.StudentId,     
			s.FirstName + ' ' + s.LastName as StudentName,     
			a.AssessmentDesc,     
			CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreCalc ELSE ass.Score END AS Score,    
			CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledProjCalc ELSE ass.Projection END AS Projection,     
			CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreProjDif ELSE ass.ScoreProjDif END AS Impact,           
			(CASE(@AssessmentType) WHEN 'F & P' THEN ass.GrowthCalc Else ass.DistrictPercentile END ) AS  DistrictPercentile ,       
			a.AssessmentId,    
			s.LastName,    
			@AssessmentType AS AssessmentType, 
			dbo.udfGetMeetExceedCriteriaValue(@DistrictId,@SubjectId,@SchoolYearId,@AssessmentTypeId,CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreProjDif ELSE ass.ScoreProjDif END)AS MeetExceedValue,
			ISNULL(ssy.LocalId,'') AS LocalId
		FROM tblStudent s    
		JOIN tblStudentSchoolYear ssy on s.StudentId = ssy.StudentId
		JOIN tblClassStudent cs ON s.StudentId = cs.StudentId    
		JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId    
		JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId    
		JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId    
		WHERE ClassId in (SELECT ClassID FROM  @tblTempClassID)    
		AND a.AssessmentTypeId = @AssessmentTypeId    
		AND a.SchoolYearId = @SchoolYearId     
		AND a.SubjectId = @SubjectId    
		AND ass.GradeLevel = @GradeLevel    
		AND ssy.SchoolYearId = @SchoolYearId
		AND s.StudentId  IN (SELECT studentId FROM @tblTempStudents)
		ORDER BY s.StudentId, st.OrderBy    
	END