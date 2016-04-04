---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename AssessmentTypeDesc to AssessmentCode and AssessmentTypeFullDescText to AssessmentTypeDesc

-- Author:  Sachin Gupta  
-- Modified date: 19/01/15  
-- Description: Add check to filter records on basis of subject related to class

-- Author:  Herdev Gangwar  
-- Modified date: 20/02/15 
-- Description: Added SubjectId check
--=====================================================================================
ALTER PROCEDURE [dbo].[spGetTeacherImpactSummary]          
      @teacherId INT,          
      @schoolId INT,          
      @schoolYearId INT,    
      @ClassId INT,    
      @ViewMeetExceedSummary BIT,
	  @tblTempStudents varchar(20)
AS          
	BEGIN          
		DECLARE @districtId INT 
		DECLARE @AssessmentTypeId INT             
  
		DECLARE @tblTempBase  TABLE    
		(   
				Grade  INT  
				,AssessmentCode VARCHAR(200)    
				,ClassSubject VARCHAR(200)  
				,AssessmentTypeId  INT  
				,Weighting  FLOAT  
				,Impact FLOAT   
				,SubjectId INT    
				,NoOfStudent  INT      
				,Average FLOAT   
				,AssessmentGradeWeightingId  INT  
				,SchoolTermId INT    
				,SortCriteria1 INT  
				,SortCriteria2 INT  
				,IsAssessmentExist BIT 
				,ReportTemplateId INT
				,AssessmentTypeDesc NVARCHAR(200)
				,ParentAssessmentId INT  
		)      
                          
		SET @districtId = (SELECT districtId FROM tblSchool WHERE SchoolId = @SchoolId)             

		INSERT INTO @tblTempBase
		SELECT DISTINCT  
			ass.GradeLevel Grade,  
			at.AssessmentCode, 
			sub.SubjectDesc SubjectDesc,  
			a.AssessmentTypeId AssessmentTypeId,  
			agw.Weighting Weighting,  
			maxterm.Impact Impact,  
			a.SubjectId SubjectId,  
			maxterm.NumOfStudent NumOfStudent,  
			maxterm.Average Average,  
			agw.AssessmentGradeWeightingId AssessmentGradeWeightingId,  
			maxterm.SchoolTermId SchoolTermId,  
			CASE(@ViewMeetExceedSummary) WHEN 0 THEN ass.GradeLevel ELSE a.SubjectId END AS SortCriteria1,  
			CASE(@ViewMeetExceedSummary) WHEN 0 THEN a.SubjectId ELSE ass.GradeLevel END AS SortCriteria2,  
			1 AS IsAssessmentExist,
			at.ReportTemplateId,
			at.AssessmentTypeDesc,
			a.ParentAssessmentId
		FROM tblAssessment A   
		JOIN tblAssessmentType at ON at.AssessmentTypeId = a.AssessmentTypeId  
		JOIN tblSubject sub ON a.SubjectId = sub.SubjectId  
		JOIN tblAssessmentScore ASS ON a.AssessmentId=ass.AssessmentId   
		JOIN tblSchool sch ON sch.SchoolId=ass.SchoolId  
		JOIN tblAssessmentWeighting aw ON aw.DistrictId = sch.DistrictId AND aw.SchoolYearId = a.SchoolYearId AND aw.AssessmentTypeId = a.AssessmentTypeId AND aw.SubjectId = a.SubjectId  
		JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId AND ass.GradeLevel = agw.Grade  
		JOIN 
		( 
			(
			  -- Note : below both query will be used to identify the number of record for having the max(SchoolTermId) value
              -- also will include all the record having union clause which do not have any value for sum(UntempASS.ScoreProjDif )
			
			  SELECT   
					termtable.AssessmentTypeId,  
					termtable.Gradelevel,
					termtable.SubjectId,  
					termtable.SchoolTermId,  
					termtable.NumofStudent,  
					termtable.Impact,  
					termtable.Average  
				FROM   
				(  
					--this query will be use to find out max value of SchoolTermId for the record which have some value in ScoreProjDif
					-- group by tempA.AssessmentTypeId,tempASS.Gradelevel, tempA.SubjectId,SchoolTermId 
					-- Note : below both query will be used to identify the number of record for having the max(SchoolTermId) value

					SELECT   
						tblA.AssessmentTypeId,  
						tblASS.Gradelevel, 
						tblA.SubjectId,  
						MAX(tbla.SchoolTermId) SchoolTermId,  
						COUNT(tblASS.studentid) NumofStudent,  
						SUM(ISNULL(tblASS.ScoreProjDif,0)) Impact,  
						(CASE COUNT(tblASS.studentid) WHEN 0 THEN 0.0 ELSE SUM(ISNULL(tblASS.ScoreProjDif,0))/COUNT(tblASS.studentid)  END) AS Average  
					FROM   
						tblAssessment tblA 
						JOIN tblAssessmentScore tblASS ON tbla.AssessmentId=tblass.AssessmentId   
						JOIN tblSchool tblsch ON tblsch.SchoolId=tblass.SchoolId
						JOIN #@tblTempStudents tempS ON tempS.StudentId = tblASS.studentid 
						JOIN tblclassstudent cs on tempS.StudentId = cs.StudentId
						JOIN tblClass tbC ON tblA.SubjectId = tbC.SubjectId AND tbC.ClassId= cs.ClassId and tbC.SchoolYearId=@schoolYearId 								
					WHERE   
						ScoreProjDif IS NOT NULL   
						AND tblsch.districtId=@districtId   
						AND tblA.SchoolYearId=@schoolYearId  
						-- Get the selected classID from the tblClass
						AND tbC.ClassId IN
							(
								SELECT c.ClassId FROM tblClass c
								JOIN tblClassTeacher tc1 ON c.ClassId = tc1.ClassId   
								WHERE 
								(@ClassId=-1 OR c.ClassId=@ClassId)
								AND (@ClassId=-1 OR c.SchoolYearId=@SchoolYearId)
								AND tc1.UserId=@teacherId 
							)
					GROUP BY   
						tblA.AssessmentTypeId,  
						tblASS.Gradelevel, 
						tblA.SubjectId,  
						tbla.SchoolTermId   
				) termtable  
				JOIN  
				(
				 -- this query will be use to find out max value of SchoolTermId for the record which have some value in ScoreProjDif
				 -- group by tempA.AssessmentTypeId,tempASS.Gradelevel, tempA.SubjectId
				 SELECT   
					tempA.AssessmentTypeId,  
					tempASS.Gradelevel, 
					tempA.SubjectId,  
					MAX(tempa.SchoolTermId) SchoolTermId   
				  FROM   
					tblAssessment tempA  
					JOIN tblAssessmentScore tempASS ON tempa.AssessmentId=tempass.AssessmentId   
					JOIN tblSchool tempsch ON tempsch.SchoolId=tempass.SchoolId
					JOIN #@tblTempStudents tempS ON tempS.StudentId = tempASS.studentid
					JOIN tblClass tempC ON tempA.SubjectId = tempC.SubjectId   
				  WHERE   
					ScoreProjDif IS NOT NULL   
					AND tempsch.districtId=@districtId    
					AND tempA.SchoolYearId=@schoolYearId  
					AND (@ClassId=-1 OR tempC.ClassId=@ClassId)
					AND (@ClassId=-1 OR tempC.SchoolYearId=@SchoolYearId)
										    
				  GROUP BY   
					tempA.AssessmentTypeId,  
					tempASS.Gradelevel,  
					tempA.SubjectId   
				 ) maxtermtable 
				ON   
					termtable.AssessmentTypeId=maxtermtable.AssessmentTypeId and   
					termtable.Gradelevel=maxtermtable.Gradelevel and
					termtable.SubjectId=maxtermtable.SubjectId and   
					termtable.SchoolTermId=maxtermtable.SchoolTermId   
				WHERE  
					termtable.SchoolTermId=maxtermtable.SchoolTermId  
			)  
			UNION  
				(
				 --//this query will be use to find out max value of SchoolTermId for the records which have null of sum(UntempASS.ScoreProjDif )
                --// group by UntempA.AssessmentTypeId,UntempASS.Gradelevel, UntempA.SubjectId  
				 SELECT   
						UntempA.AssessmentTypeId,  
						UntempASS.Gradelevel,
						UntempA.SubjectId,  
						max(Untempa.SchoolTermId) SchoolTermId,  
						0 AS NumofStudent,  
						0.0 AS Impact,  
						0.0 AS Average   
				FROM   
					tblAssessment UntempA  
					JOIN tblAssessmentScore UntempASS ON Untempa.AssessmentId=Untempass.AssessmentId   
					JOIN tblSchool Untempsch ON Untempsch.SchoolId=Untempass.SchoolId
					JOIN #@tblTempStudents tempS ON tempS.StudentId = UntempASS.studentid
					JOIN tblclassstudent cs on tempS.StudentId = cs.StudentId
					JOIN tblClass tbC ON UntempA.SubjectId = tbC.SubjectId AND tbC.ClassId= cs.ClassId and tbC.SchoolYearId=@schoolYearId 
                    
				WHERE   
					Untempsch.districtId=@districtId    
					AND UntempA.SchoolYearId=@schoolYearId  
					-- Get the selected classID from the tblClass
					AND tbC.ClassId IN
						(
							SELECT c.ClassId FROM tblClass c
							JOIN tblClassTeacher tc1 ON c.ClassId = tc1.ClassId   
							WHERE 
							(@ClassId=-1 OR c.ClassId=@ClassId)
							AND (@ClassId=-1 OR c.SchoolYearId=@SchoolYearId)
							AND tc1.UserId=@teacherId 
						)  

				GROUP BY   
					UntempA.AssessmentTypeId,  
					UntempASS.Gradelevel,  
					UntempA.SubjectId  
					HAVING SUM(UntempASS.ScoreProjDif) IS NULL
				)     
		) maxterm   
		ON   
			A.SchoolTermId=maxterm.SchoolTermId AND   
			maxterm.AssessmentTypeId=a.AssessmentTypeId AND   
			maxterm.Gradelevel=ass.GradeLevel AND  
			maxterm.SubjectId =a.SubjectId   
		WHERE   
			sch.DistrictId=@DistrictId
			AND a.SchoolYearId=@schoolYearId
			AND ass.studentid IN
			(
				SELECT st.studentid FROM TBLCLASSSTUDENT st   
				JOIN #@tblTempStudents tempst ON st.studentid=tempst.studentid 	
				JOIN tblClass c ON c.ClassId = st.ClassId and c.SubjectId =a.SubjectId 
					AND c.SchoolYearId =  @schoolYearId  						
				JOIN tblClassTeacher tc1 ON c.ClassId = tc1.ClassId   
				WHERE tc1.UserId=@teacherId  AND   
					(@ClassId=-1 OR c.ClassId=@ClassId)
					AND (@ClassId=-1 OR c.SchoolYearId=@SchoolYearId)
			)               
					  
	-- Get the records whose assessment is not created but have assessment waiting.
	IF EXISTS(SELECT TOP 1 * FROM @tblTempBase)
	BEGIN
		INSERT INTO 
		@tblTempBase
		(Grade
		,SubjectId
		,AssessmentTypeId
		,AssessmentCode
		,ClassSubject
		,Weighting
		,AssessmentGradeWeightingId
		,SortCriteria1
		,SortCriteria2
		,IsAssessmentExist
		,ReportTemplateId
		,AssessmentTypeDesc
		,ParentAssessmentId)  
		SELECT   DISTINCT 
			tblbase.grade
			,tblbase.subjectid
			,aw.AssessmentTypeId
			,at.AssessmentCode
			,tblbase.ClassSubject
			,agw.Weighting
			,agw.AssessmentGradeWeightingId
			,tblbase.SortCriteria1
			,tblbase.SortCriteria2
			,0
			,at.ReportTemplateId
			,at.AssessmentTypeDesc
			,tblbase.ParentAssessmentId  
		FROM tblAssessmentWeighting aw   
		JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId   
		JOIN tblAssessmentType at ON at.AssessmentTypeId = aw.AssessmentTypeId
		JOIN @tblTempBase tblbase ON aw.subjectid = tblbase.subjectid AND agw.grADE =tblbase.grade  AND   
		aw.DISTRICTID = @DistrictId AND aw.schoolyearid = @schoolYearId     
		WHERE  
			NOT EXISTS 
			(
				SELECT t.AssessmentTypeId 
				FROM @tblTempBase t 
				WHERE 
					t.subjectid= tblbase.subjectid and 
					t.grADE = tblbase.grade and  
					t.AssessmentTypeId= aw.AssessmentTypeId
			)
	END  
              
	SELECT    
		Grade  
		,AssessmentCode   
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
		,ReportTemplateId
		,AssessmentTypeDesc 
		,ParentAssessmentId
	FROM @tblTempBase
	WHERE (
			Weighting > 0 
			OR
			(Weighting=0 AND ParentAssessmentId IS NULL)
		  )
	ORDER BY SortCriteria1,SortCriteria2, Weighting DESC,AssessmentCode  
              
END