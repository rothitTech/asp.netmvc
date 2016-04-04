IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spGetTeacherImpactDetailAssessmentList')
BEGIN
	DROP PROCEDURE spGetTeacherImpactDetailAssessmentList
END	
GO  
-- =============================================              
-- Author:  <Author,,Name>              
-- Create date: <Create Date,,>              
-- Description: <Description,,>        
      
-- Modified by: Hardev Gangwar        
 -- Modified dt: 09/12/2014                  
-- Description: if class ID is not -1 then filter Teacher Impact Detail Assessment on class id       

-- Modified by: Vikrant Arya        
-- Modified dt: 10/13/2014                  
-- Description: Include the School term id in the select statement        
            

-- spGetTeacherImpactDetailAssessmentList 1,3,3,17,3,-1        
-- =============================================   
CREATE PROCEDURE [dbo].[spGetTeacherImpactDetailAssessmentList]               
   @SubjectId INT,              
   @SchoolYearId INT,              
   @AssessmentTypeId INT,              
   @TeacherId INT,              
   @GradeLevel INT,          
   @ClassID  INT          
AS            
BEGIN          
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
            
	BEGIN              
		SELECT DISTINCT 
			a.AssessmentDesc
			,a.AssessmentId
			,st.SchoolTermId
			,st.OrderBy              
		FROM tblStudent s              
		JOIN tblClassStudent cs ON s.StudentId = cs.StudentId              
		JOIN tblAssessmentScore ass ON cs.StudentId = ass.StudentId              
		JOIN tblAssessment a ON ass.AssessmentId = a.AssessmentId              
		JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId              
		WHERE ClassId IN ( SELECT ClassID FROM  @tblTempClassID)              
		AND a.AssessmentTypeId = @AssessmentTypeId              
		AND a.SchoolYearId = @SchoolYearId               
		AND a.SubjectId = @SubjectId              
		ORDER BY ST.OrderBy              
	END 
END	