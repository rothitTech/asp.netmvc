USE [dbTIR]
GO
/****** Object:  StoredProcedure [dbo].[spGetTeacherImpactSummary]    Script Date: 1/19/2015 12:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--===================================================
-- Author:  Sachin Gupta  
-- Modified date: 29/12/14  
-- Description: Rename AssessmentTypeDesc to AssessmentCode and AssessmentTypeFullDescText to AssessmentTypeDesc

-- Author:  Sachin Gupta  
-- Modified date: 19/01/15  
-- Description: Added check to filter records on basis of subject related to class
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
            )      
                          
            SET @districtId = (SELECT districtId FROM tblSchool WHERE SchoolId = @SchoolId)             
			INSERT INTO @tblTempBase
				   
                        SELECT   
                        DISTINCT  
                        ass.GradeLevel Grade,  
                        at.AssessmentCode, 
                        sub.SubjectDesc SubjectDesc,  
                        a.AssessmentTypeId AssessmentTypeId,  
                        agw.Weighting Weighting,  
                        maxterm.Impact Impact,  
                        a.SubjectId SubjectId,  
                        maxterm.NumOfStudent  NumOfStudent,  
                        maxterm.Average Average,  
                        agw.AssessmentGradeWeightingId AssessmentGradeWeightingId,  
                        maxterm.SchoolTermId SchoolTermId,  
                        CASE(@ViewMeetExceedSummary) WHEN 0 THEN ass.GradeLevel ELSE a.SubjectId END AS SortCriteria1,  
                        CASE(@ViewMeetExceedSummary) WHEN 0 THEN a.SubjectId ELSE ass.GradeLevel END AS SortCriteria2,  
                        1 AS IsAssessmentExist,
						at.ReportTemplateId,
						at.AssessmentTypeDesc
                        FROM tblAssessment A   
                        JOIN tblAssessmentType at ON at.AssessmentTypeId = a.AssessmentTypeId  
                        JOIN tblSubject sub ON a.SubjectId = sub.SubjectId  
                        JOIN tblAssessmentScore ASS ON a.AssessmentId=ass.AssessmentId   
                        JOIN tblSchool sch ON sch.SchoolId=ass.SchoolId  
                        JOIN tblAssessmentWeighting aw ON aw.DistrictId = sch.DistrictId AND aw.SchoolYearId = a.SchoolYearId AND aw.AssessmentTypeId = a.AssessmentTypeId AND aw.SubjectId = a.SubjectId  
						JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId AND ass.GradeLevel = agw.Grade  
                        JOIN ( (SELECT   
                               termtable.AssessmentTypeId,  
                               termtable.Gradelevel,
                               termtable.SubjectId,  
                               termtable.SchoolTermId,  
                               termtable.NumofStudent,  
                               termtable.Impact,  
                               termtable.Average  
								 FROM   
								 (  
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
									WHERE   
									ScoreProjDif IS NOT NULL   
									AND tblsch.districtId=@districtId   
									AND tblA.SchoolYearId=@schoolYearId  
									AND tblA.SubjectId  IN
									(
										SELECT SubjectId FROM tblClass WHERE ClassId = CASE WHEN @ClassId = -1 THEN ClassId  ELSE @ClassId END
										AND SchoolYearId = CASE WHEN @ClassId = -1 THEN @SchoolYearId  ELSE @SchoolYearId END
									)
									GROUP BY   
                                    tblA.AssessmentTypeId,  
                                    tblASS.Gradelevel, 
                                    tblA.SubjectId,  
                                    tbla.SchoolTermId   
                                ) termtable  
						 JOIN  
							(SELECT   
								tempA.AssessmentTypeId,  
								tempASS.Gradelevel, 
								tempA.SubjectId,  
								MAX(tempa.SchoolTermId) SchoolTermId   
                            FROM   
								tblAssessment tempA  
								JOIN tblAssessmentScore tempASS ON tempa.AssessmentId=tempass.AssessmentId   
								JOIN tblSchool tempsch ON tempsch.SchoolId=tempass.SchoolId
								JOIN #@tblTempStudents tempS ON tempS.StudentId = tempASS.studentid
                            WHERE   
								ScoreProjDif IS NOT NULL   
								AND tempsch.districtId=@districtId    
								AND tempA.SchoolYearId=@schoolYearId  
								AND tempA.SubjectId  IN
								(
									SELECT SubjectId FROM tblClass WHERE ClassId = CASE WHEN @ClassId = -1 THEN ClassId  ELSE @ClassId END
									AND SchoolYearId = CASE WHEN @ClassId = -1 THEN @SchoolYearId  ELSE @SchoolYearId END
								)		    
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
                        (SELECT   
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
                        WHERE   
							Untempsch.districtId=@districtId    
							AND UntempA.SchoolYearId=@schoolYearId    
							AND UntempA.SubjectId  IN
							(
								SELECT SubjectId FROM tblClass WHERE ClassId = CASE WHEN @ClassId = -1 THEN ClassId  ELSE @ClassId END
								AND SchoolYearId = CASE WHEN @ClassId = -1 THEN @SchoolYearId  ELSE @SchoolYearId END
							) 
                        GROUP BY   
							UntempA.AssessmentTypeId,  
							UntempASS.Gradelevel,  
							UntempA.SubjectId  
							HAVING SUM(UntempASS.ScoreProjDif) IS NULL)     
                        ) maxterm   
                        ON   
							A.SchoolTermId=maxterm.SchoolTermId AND   
							maxterm.AssessmentTypeId=a.AssessmentTypeId AND   
							maxterm.Gradelevel=ass.GradeLevel AND  
							maxterm.SubjectId =a.SubjectId   
                        WHERE   
							sch.DistrictId=@DistrictId AND   
							a.SchoolYearId=@schoolYearId AND 
							ass.studentid IN(SELECT st.studentid FROM TBLCLASSSTUDENT st   
							JOIN #@tblTempStudents tempst ON st.studentid=tempst.studentid  
							JOIN tblClass c ON c.ClassId = st.ClassId 							
							AND c.SchoolYearId =  @schoolYearId    
							JOIN tblClassTeacher tc1 ON c.ClassId = tc1.ClassId   
                        WHERE tc1.UserId=@teacherId  AND   
							c.ClassId = CASE WHEN @ClassId = -1 THEN c.ClassId  ELSE @ClassId END AND  
							c.SchoolYearId = CASE WHEN @ClassId = -1 THEN @SchoolYearId  ELSE c.SchoolYearId END   
						)               
					  
					IF EXISTS(SELECT TOP 1 * FROM @tblTempBase)
							BEGIN
											  INSERT INTO @tblTempBase(Grade,SubjectId,AssessmentTypeId,AssessmentCode,ClassSubject,Weighting,AssessmentGradeWeightingId,SortCriteria1,SortCriteria2,IsAssessmentExist,ReportTemplateId,AssessmentTypeDesc)  
											  SELECT   DISTINCT 
											  tblbase.grade,tblbase.subjectid,aw.AssessmentTypeId,at.AssessmentCode,
											  tblbase.ClassSubject,agw.Weighting,agw.AssessmentGradeWeightingId,
											  tblbase.SortCriteria1,tblbase.SortCriteria2,0, at.ReportTemplateId, at.AssessmentTypeDesc  
											  FROM tblAssessmentWeighting aw   
											  JOIN tblAssessmentGradeWeighting agw ON agw.AssessmentWeightingId = aw.AssessmentWeightingId   
											  JOIN tblAssessmentType at ON at.AssessmentTypeId = aw.AssessmentTypeId
											  JOIN @tblTempBase tblbase ON aw.subjectid = tblbase.subjectid AND agw.grADE =tblbase.grade  AND   
											  aw.DISTRICTID = @DistrictId AND aw.schoolyearid = @schoolYearId     
											  WHERE  NOT EXISTS (SELECT t.AssessmentTypeId FROM @tblTempBase t 
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
            FROM @tblTempBase   
            ORDER BY SortCriteria1,SortCriteria2, Weighting DESC,AssessmentCode  
              
      END
