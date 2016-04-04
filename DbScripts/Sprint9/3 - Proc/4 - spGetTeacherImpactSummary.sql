-- =============================================        
-- Author:  <Author,,Name>        
-- Create date: <Create Date,,>        
-- Description: <Description,,>        
  
-- =============================================        
-- Modified by: Vikrant Arya                    
-- Modified date : 29/09/2014                    
-- US-115 changes : Do not include the student in student count, if the score proj difference is null        
  
-- Modified by: Vikrant Arya                    
-- Modified date : 09/10/2014                    
-- US-151 changes : Modify to get the school term id       
   
-- =============================================   
-- Modified by: Sachin Gupta                   
-- Modified date : 10/16/2014                    
-- Description : Modified sp to allow conditional sorting on basis of ViewMeetExceedSummary flag as per US-160    

-- Modified by: Hardev Gangwar                   
-- Modified date : 10/30/2014                    
-- Description : Modified sp to allow get the assessment type which does not have assessment and score but have weighting as per US-146   

-- Modified by: Hardev Gangwar                   
-- Modified date : 11/14/2014                    
-- Description : Modified sp to allow TeacherId, SchoolId and ClassId in UDF udfDistrictsLastAssessmentsTable as per US-135

-- Modified by: Vikrant Arya                   
-- Modified date : 11/19/2014                    
-- Description : Includes the report filters as per US-202

-- Modified by: Vikrant Arya                   
-- Modified date : 11/21/2014                    
-- Description : Filter student against the selected report filters

-- exec spGetTeacherImpactSummary 447, 1, 3,-1 , 1

--Drop function if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='P' AND name = 'spGetTeacherImpactSummary')
BEGIN
	DROP PROCEDURE spGetTeacherImpactSummary
END 
GO
  
CREATE PROCEDURE [dbo].[spGetTeacherImpactSummary]        
	@teacherId int,        
	@schoolId int,        
	@schoolYearId int,  
	@ClassId INT,  
	@ViewMeetExceedSummary BIT
AS        
	BEGIN        
		DECLARE @districtId INT,  
				@Counter INT,
				@Count INT,
				@grade INT,
				@subjectid INT,
				@SubjectDesc VARCHAR(200),
				@SchoolTermId INT,
				@SortCriteria1 INT,
				@SortCriteria2	INT
				
		-- Use Table Variable for class filter      
		DECLARE @tblTempClassID table    
		(    
			ID INT IDENTITY(1,1),    
			ClassId INT    
		)  
		
		DECLARE @tblTempStudent table
		(
			CounterId INT IDENTITY(1,1), 
			StudentId int not null
		)

		DECLARE @tblTempBase  TABLE  
		( 
			 Grade  INT
			,AssessmentType VARCHAR(200)  
			,ClassSubject VARCHAR(200)
			,AssessmentTypeId  INT
			,Weighting  DECIMAL (6,3)
			,Impact DECIMAL (6,3) 
			,SubjectId INT  
			,NoOfStudent  INT    
			,Average DECIMAL (6,3) 
			,AssessmentGradeWeightingId  INT
			,SchoolTermId INT  
			,SortCriteria1 INT
			,SortCriteria2 INT
			,IsAssessmentExist BIT
		 )    
   		
   		--Execute Report filter stored proc to get the filter students
   		IF NOT EXISTS(SELECT [name] FROM tempdb.sys.tables WHERE [name] like '#tblTempStudents%')
		BEGIN
			INSERT INTO @tblTempStudent (StudentId)
			SELECT S.STUDENTID FROM tblStudent S 
			INNER JOIN tblStudentSchoolYear SSY ON S.StudentId = SSY.StudentId 
			WHERE SSY.SchoolYearId = @schoolYearId
		END
		ELSE
		BEGIN
			INSERT INTO @tblTempStudent (StudentId)
			SELECT StudentId FROM #tblTempStudents
		END
   		
   		DECLARE @studentCounter INT,	
   				@studentCount INT,
   				@studentIds VARCHAR(MAX),
   				@studentId INT
   		
		SET @studentCounter = 1
   		SELECT @studentCount = Count(*) FROM @tblTempStudent		
   		
   		WHILE( @studentCounter <= @studentCount)  
		BEGIN 
			SET @studentId = NULL
   			SELECT @studentId = StudentId FROM @tblTempStudent WHERE CounterId = @studentCounter
			SET @studentIds =  COALESCE(@studentIds + ',', '') + CAST(@studentId AS VARCHAR(MAX))
   			SET @studentCounter = @studentCounter + 1
   		END
			     
		IF (@ClassId = -1)     
		BEGIN         
			INSERT INTO @tblTempClassID(ClassID)         
			SELECT tc1.ClassId        
			FROM tblClassTeacher tc1
			INNER JOIN tblClass c ON c.ClassId = tc1.ClassId        
			WHERE tc1.UserId = @teacherId AND c.SchoolYearId = @SchoolYearId    
		END        
		ELSE        
		BEGIN          
			INSERT INTO @tblTempClassID(ClassID)        
		    SELECT @ClassId      
		END   
		SET @districtId = (SELECT districtId FROM tblSchool WHERE SchoolId = @SchoolId)
		INSERT INTO @tblTempBase 
				SELECT
				base.Grade
				,base.AssessmentType
				,base.ClassSubject
				,base.AssessmentTypeId
				,agw.Weighting
				,base.Impact
				,base.SubjectId
				,base.NoOfStudent
				,(CASE NoOfStudent WHEN 0 THEN 0.0 ELSE base.Impact/base.NoOfStudent END) AS Average
				,agw.AssessmentGradeWeightingId
				,base.SchoolTermId
				,case(@ViewMeetExceedSummary) when 0 then base.Grade else base.SubjectId end as SortCriteria1
				,case(@ViewMeetExceedSummary) when 0 then base.SubjectId else base.Grade end as SortCriteria2
				,1 AS IsAssessmentExist
				FROM
				(SELECT 
					s.DistrictId
					,a.SchoolYearId
					,sc.GradeLevel AS Grade
					,a.SubjectId
					,a.AssessmentTypeId
					,at.AssessmentTypeDesc AS AssessmentType
					,sub.SubjectDesc AS ClassSubject
					,SUM(ISNULL(sc.ScaledScoreProjDif,0)) AS Impact
					--,COUNT(cs2.StudentId) AS NoOfStudent
					,SUM(CASE WHEN sc.ScaledScoreProjDif IS NULL THEN 0 ELSE 1 END) AS NoOfStudent
					,a.SchoolTermId
				FROM tblAssessment a
				JOIN tblAssessmentScore sc ON sc.AssessmentId = a.AssessmentId
				-- and sc.SchoolID = @schoolId  --Code Uncommented for US 73
				JOIN tblSubject sub ON a.SubjectId = sub.SubjectId 
				JOIN tblAssessmentType at ON at.AssessmentTypeId = a.AssessmentTypeId
				JOIN tblschool s ON s.SchoolId = sc.SchoolID   
				JOIN (SELECT AssessmentId, GradeLevel FROM [dbo].[udfDistrictsLastAssessmentsTable](@districtId, @schoolYearId, @teacherId,@ClassId, @studentIds)) as lastAssessment
						on lastAssessment.AssessmentId = sc.AssessmentId and lastAssessment.GradeLevel = sc.gradelevel
				JOIN  (	SELECT DISTINCT --Apply distinct keyword for US 108
							cs1.StudentId
							--,c.Grade --Code commented for US 108
							,c.SubjectId 
						FROM tblClassStudent cs1
						JOIN tblClass c ON c.ClassId = cs1.ClassId
						WHERE cs1.ClassId IN ( SELECT ClassID FROM  @tblTempClassID)
						AND c.SchoolYearId =  @schoolYearId
						) AS cs2 
						ON cs2.StudentId = sc.StudentId  
				-- and cs2.Grade = sc.GradeLevel   --Code Uncommented for US 73
				AND cs2.SubjectId = a.SubjectId 
				WHERE cs2.StudentId  IN (SELECT studentId FROM @tblTempStudent)
				GROUP BY s.DistrictId, a.SchoolYearId, sc.GradeLevel, a.SubjectId, a.AssessmentTypeId, at.AssessmentTypeDesc, sub.SubjectDesc,a.SchoolTermId
				) AS base
				JOIN tblAssessmentWeighting aw ON aw.DistrictId = base.DistrictId AND aw.SchoolYearId = base.SchoolYearId
				AND aw.AssessmentTypeId = base.AssessmentTypeId AND aw.SubjectId = base.SubjectId
				JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId AND base.Grade = agw.Grade
		
		IF(SELECT COUNT(*) FROM @tblTempBase)>0
		 BEGIN
			DECLARE @tblTempGradeSubject  TABLE  
			 (  
				CounterId INT IDENTITY(1,1)     
				,Grade INT  
				,SubjectId INT 
				,SubjectDesc VARCHAR(200)
				,SortCriteria1 INT
				,SortCriteria2 INT
			 )  
			INSERT INTO @tblTempGradeSubject(Grade, SubjectId,SubjectDesc,SortCriteria1,SortCriteria2)
			SELECT DISTINCT Grade, subjectid,ClassSubject,SortCriteria1,SortCriteria2 FROM @tblTempBase 
			 
			SET @Count = null
			SELECT @Count = COUNT(*) FROM @tblTempGradeSubject  
			SET @Counter = 1     
			WHILE( @Counter <= @Count)  
			BEGIN  
					SET @grade =null
					SET @subjectid=null
					SET @SubjectDesc =null
					SET @SchoolTermId =null	
					SET @SortCriteria1 =null
					SET @SortCriteria2=null	
					
					SELECT  @grade = grade, @subjectid = subjectid, @SubjectDesc=SubjectDesc, @SortCriteria1=SortCriteria1,@SortCriteria2=SortCriteria2
					FROM @tblTempGradeSubject WHERE CounterId = @Counter

					INSERT INTO @tblTempBase(Grade,SubjectId,AssessmentTypeId,AssessmentType,ClassSubject,Weighting,AssessmentGradeWeightingId,SortCriteria1,SortCriteria2,IsAssessmentExist)

					SELECT  
					@grade,@subjectid,aw.AssessmentTypeId,at.AssessmentTypeDesc,@SubjectDesc,agw.Weighting,agw.AssessmentGradeWeightingId,@SortCriteria1,@SortCriteria2,0
					FROM tblAssessmentWeighting aw 
					JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId 
					JOIN tblAssessmentType at ON at.AssessmentTypeId = aw.AssessmentTypeId   

					WHERE aw.subjectid = @subjectid and agw.grADE =@grade  AND 
					aw.DISTRICTID = @districtId and aw.schoolyearid = @schoolYearId 
					and aw.AssessmentTypeId not in 
					(SELECT t.AssessmentTypeId FROM @tblTempBase t WHERE subjectid= @subjectid and grADE = @grade and DISTRICTID = @districtId and schoolyearid = @schoolYearId )
					SET @Counter = @Counter + 1
			END
		END
		
		SELECT  
			Grade
			,AssessmentType 
			,ClassSubject 
			,AssessmentTypeId
			,Weighting 
			,Impact 
			,SubjectId 
			,NoOfStudent  
			,Average 
			,AssessmentGradeWeightingId 
			,SchoolTermId 
			,SortCriteria1 
			,SortCriteria2 
			,IsAssessmentExist 
		FROM @tblTempBase 
		ORDER BY SortCriteria1,SortCriteria2, Weighting DESC,AssessmentType
	END 
	
	
	

