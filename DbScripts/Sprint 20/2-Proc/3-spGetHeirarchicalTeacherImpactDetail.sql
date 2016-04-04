--exec spGetHeirarchicalTeacherImpactDetail 7,3,11,463,3,0, -1, 1,1
ALTER PROCEDURE [dbo].[spGetHeirarchicalTeacherImpactDetail]     
	@SubjectId INT,    
    @SchoolYearId INT,    
    @AssessmentTypeId INT,    
    @TeacherId INT,    
    @GradeLevel INT,         
    @ViewScaledScore BIT,    
    @ClassID  INT ,    
    @DistrictId INT,
	@InputTermId INT,
    @Race INT = NULL,
	@Gender INT = NULL,
	@FrlIndicator BIT = NULL,
	@IepIndicator BIT = NULL,
	@LepIndicator BIT = NULL,
	@Hispanic BIT = NULL,   
	@InputParentAssessmentTypeId INT = NULL
	
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
	WHERE SSY.SchoolYearId = @schoolYearId 
	AND SSY.GradeLevel = @GradeLevel
	AND SSY.ServingSchoolId in (select schoolId from tblSchool where DistrictId = @DistrictId)
	AND (@LepIndicator IS NULL OR SSY.LepIndicator = @LepIndicator)
	AND (@IepIndicator IS NULL OR SSY.IepIndicator = @IepIndicator)
	AND (@FrlIndicator IS NULL OR SSY.FrlIndicator = @FrlIndicator)
	AND (@Hispanic IS NULL OR SSY.Hispanic = @Hispanic)
	AND (@Race IS NULL OR S.RaceId = @Race)
	AND (@Gender IS NULL OR S.GenderId = @Gender)
    
	
	DECLARE @parentAssessmentTypeId INT
	SET @parentAssessmentTypeId = @InputParentAssessmentTypeId
	IF @InputParentAssessmentTypeId IS NULL
	BEGIN
	SELECT @parentAssessmentTypeId = at.AssessmentTypeId 
	     FROM tblAssessmentType at
		 JOIN tblAssessment a ON at.AssessmentTypeId = a.AssessmentTypeId
		 WHERE at.AssessmentTypeId = @AssessmentTypeId AND SchoolTermId = @InputTermId
	END

    DECLARE @typeId INT
	SET @typeId = @InputParentAssessmentTypeId
	IF @InputParentAssessmentTypeId IS NULL
	BEGIN
	SELECT @typeId =  @AssessmentTypeId 
	END


    BEGIN    
		SELECT 
			s.StudentId,     
			s.FirstName + ' ' + s.LastName as StudentName,     
			a.AssessmentDesc,     
			ass.Score AS Score,    
			ass.Projection AS Projection,     
			ass.ScoreProjDif AS Impact,           
			a.AssessmentId,   
			a.AssessmentTypeId,  
			s.LastName,    
			dbo.udfGetMeetExceedCriteriaValue(@DistrictId,@SubjectId,@SchoolYearId,a.AssessmentTypeId,ass.ScoreProjDif)AS MeetExceedValue,
			ISNULL(ssy.LocalId,'') AS LocalId,
			ass.DistrictPercentile,
			ass.GrowthCalc,
			at.ParentAssessmentTypeId,
			a.SchoolTermId,
			a.AssessmentId,
			at.AssessmentCode,
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
		AND (at.AssessmentTypeId = @typeId
			OR at.ParentAssessmentTypeId = @parentAssessmentTypeId)	
		AND a.SchoolYearId = @SchoolYearId     
		AND a.SubjectId = @SubjectId  
		AND c.SubjectId = @SubjectId   
		AND ass.GradeLevel = @GradeLevel    
		AND ssy.SchoolYearId = @SchoolYearId
		AND s.StudentId  IN (SELECT studentId FROM @tblTempStudents)
		AND a.SchoolTermId = @InputTermId
		ORDER BY 
			s.StudentId -- Order by 'StudentId'
			,st.OrderBy -- Order by column 'OrderBy' means for each school term there is an order defined.
			,at.ParentAssessmentTypeId -- Order by 'ParentAssessmentId' means to keep ParentId at top from childerns for an assessment type.
			,a.AssessmentTypeId	-- Order by AssessmentTypeId to get same order of accessments for all students.
	END