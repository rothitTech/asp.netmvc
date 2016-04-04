    
-- Author:  Henry Schwenk    
-- Create date: <Create Date,,>        
-- Modified dt: 7/29/14 (Vikrant)      
-- Description: spGetTeacherImpactDetail         
    
-- Modified by: Sachin Gupta       
-- Modified date : 01/09/2014       
-- Changes done as per US-93 (In table tblAssessmentScore, Score column is made null and If Score is Null then ScaledScoreProjDiff and ScoreProjDiff        
-- should also be null )                
               
-- Modified by: Hardev Gangwar              
-- Modified dt: 09/12/2014         
-- Description: if class ID is not -1 then filter TeacherImpactDetail on class id                  
           
-- Modified by: Irabanta    
-- Modified date: 10/1/2014    
-- Description: story 140 to bring up LastName and default sorting on lastname. Also refactor @flag to @ViewScaledScore     
  
-- Modified by: Hardev Gangwar    
-- Modified date: 10/07/2014    
-- Description: story 139 return MeetExceedCriteriaValue on the basisi of SubjectId, SchoolYearId,AssessmentTypeId and DistrictId  
    
-- Modified by: Vikrant Arya    
-- Modified date: 10/22/2014    
-- Description: US 172 changes to include student's localId column  

-- Modified by: Vikrant Arya    
-- Modified date: 11/05/2014    
-- Description: US 186 changes to pickup LocalId from tblstudentschoolyear table      

-- Modified by: Vikrant Arya                   
-- Modified date : 11/21/2014                    
-- Description : Filter student against the selected report filters

-- Modified by: Sachin Gupta                   
-- Modified date : 11/25/2014                    
-- Description : Fixed Issue as M/E arrow was not coming correct for F&P assessment.
-- =========================================================================================    
-- Exec spGetTeacherImpactDetail 1,3,2,469,3,1,-1 ,1  

--Drop a proc if exists in the system
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
		AND c.SubjectId = @SubjectId        
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
          
	SET @Query = 'SELECT S.STUDENTID FROM tblStudent S 
				  INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
				  WHERE SSY.SchoolYearId = ' + CONVERT(VARCHAR(10), @schoolYearId) 

	IF(@LepIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.LepIndicator = ' + CONVERT(VARCHAR(10), @LepIndicator)
	END

	IF(@IepIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.IepIndicator = ' + CONVERT(VARCHAR(10), @IepIndicator)
	END

	IF(@FrlIndicator IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.FrlIndicator = ' +  CONVERT(VARCHAR(10), @FrlIndicator)
	END

	IF(@Hispanic IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND SSY.Hispanic = ' +  CONVERT(VARCHAR(10), @Hispanic)
	END

	IF(@Race IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND S.RaceId = ' +  CONVERT(VARCHAR(10), @Race)
	END

	IF(@Gender IS NOT NULL)
	BEGIN
		SET @Query = @Query + 'AND S.GenderId = ' +  CONVERT(VARCHAR(10), @Gender)
	END

	--Retrieving students for the report filter
	INSERT INTO @tblTempStudents(studentId)
	EXEC (@Query)
    
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

