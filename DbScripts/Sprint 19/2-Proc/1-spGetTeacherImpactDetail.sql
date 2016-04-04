-- ============================================= 
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename column AssessmentTypeDesc to AssessmentCode     

-- Author:  Sachin Gupta  
-- Modified date: 23/01/15 
-- Description: Replaced AssessmentCode check for F&P case.

-- Author:  Herdev Gangwar  
-- Modified date: 20/02/15 
-- Description: Added SubjectId check
-- =================================================================================    
-- exec spGetTeacherImpactDetail 2,5,19,67,4,0,-1, 1
ALTER PROCEDURE [dbo].[spGetTeacherImpactDetail]     
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
    
	DECLARE @ReportTemplateId INT,   
			@Query VARCHAR(MAX)

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
		WHERE c.SchoolYearId = @SchoolYearId             
		AND ct.UserId = @TeacherId    
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
	AND SSY.GradeLevel = @GradeLevel
	AND SSY.ServingSchoolId in (select schoolId from tblSchool where DistrictId = @DistrictId)
	AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
	AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
	AND (@FrlIndicator IS NULL OR SSY.FrlIndicator = @FrlIndicator)
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
			a.AssessmentId,    
			s.LastName,    
			dbo.udfGetMeetExceedCriteriaValue(@DistrictId,@SubjectId,@SchoolYearId,@AssessmentTypeId,CASE(@ViewScaledScore) WHEN 1 THEN ass.ScaledScoreProjDif ELSE ass.ScoreProjDif END)AS MeetExceedValue,
			ISNULL(ssy.LocalId,'') AS LocalId,
			ass.DistrictPercentile,
			ass.GrowthCalc,
			asts.TemplateFileName as SLOFileName,
			astr.TemplateFileName as RubricFileName
		FROM tblStudent s    
		JOIN tblStudentSchoolYear ssy on s.StudentId = ssy.StudentId
		JOIN tblClassStudent cs ON s.StudentId = cs.StudentId    
		JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId    
		JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId    
		JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId   
		JOIN tblAssessmentType at ON a.AssessmentTypeId = at.AssessmentTypeId  
		JOIN tblClass c on cs.ClassId = c.ClassId
		LEFT JOIN tblAssessmentTemplate asts ON a.SLOTemplateId = asts.AssessmentTemplateId 
		LEFT JOIN tblAssessmentTemplate astr ON  a.RubricTemplateId = astr.AssessmentTemplateId
        WHERE c.ClassId in (SELECT ClassID FROM  @tblTempClassID)   
		AND a.AssessmentTypeId = @AssessmentTypeId    
		AND a.SchoolYearId = @SchoolYearId     
		AND a.SubjectId = @SubjectId  
		AND c.SubjectId = @SubjectId   
		AND ass.GradeLevel = @GradeLevel    
		AND ssy.SchoolYearId = @SchoolYearId
		AND s.StudentId  IN (SELECT studentId FROM @tblTempStudents)
		ORDER BY s.StudentId, st.OrderBy    
	END