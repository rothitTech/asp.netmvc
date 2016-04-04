
-- Author:		Henry Schwenk
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

-- Modified By :Hardev Gangwar
-- Modified date: 11/12/2014
-- Description:	pick up the last assessment where ScaledScoreProjDiff is not null
-- select * from udfDistrictsLastAssessmentsTable(1,5)

-- Modified By :Vikrant Arya
-- Modified date: 11/20/2014
-- Description:	Apply the schoolyear filter on selection of classes


--Drop function if exists in the system
IF EXISTS (SELECT * FROM sys.objects WHERE type ='TF' AND name = 'udfDistrictsLastAssessmentsTable')
BEGIN
	DROP Function udfDistrictsLastAssessmentsTable
END 
GO

CREATE FUNCTION [dbo].[udfDistrictsLastAssessmentsTable]
(
	@DistrictId INT,
	@SchoolYearId INT,
	@teacherId INT,
	@ClassId INT,
	@StudentIds VARCHAR(MAX)
)

RETURNS @DistrictsLastAssessmentsTable TABLE 
(
	AssessmentId int NOT NULL, 
	AssessmentTypeId int NOT NULL, 
	SubjectId int NOT NULL,
	GradeLevel int NULL
)
AS
BEGIN
	-- Use Table Variable for class filter      
	DECLARE @tblTempClassID table    
	(    
		ID INT IDENTITY(1,1),    
		ClassId INT    
	)
	
	DECLARE @tblTempStudent table
	(
		StudentId int not null
	)
	
	DECLARE @delimiter VARCHAR(1)
	SET @delimiter = ','

	DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @StudentIds)

    WHILE @start < LEN(@StudentIds) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@StudentIds) + 1

        INSERT INTO @tblTempStudent (StudentId) 
        VALUES(SUBSTRING(@StudentIds, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @StudentIds, @start)
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
	    
	DECLARE @BaseTemp TABLE
	(
		  AssessmentId int NOT NULL,
		  RowNumber int not null,
		  AssessmentTypeId int NOT NULL,
		  SubjectId int NOT NULL,
		  GradeLevel int null,
		  Flag bit,
		  Orderby int not null      
	)

	DECLARE @ChildTemp TABLE
	(
		  AssessmentId int NOT NULL,
		  RowNumber int not null,
		  AssessmentTypeId int NOT NULL,
		  SubjectId int NOT NULL,
		  GradeLevel int null,
		  Flag bit,
		  Orderby int not null,
		  RowNumberNew int not null
	)

	INSERT INTO @BaseTemp (AssessmentId,RowNumber,AssessmentTypeId,SubjectId,GradeLevel,Flag,Orderby)
	SELECT 
	scr.AssessmentId ,
	ROW_NUMBER() OVER (PARTITION BY a.AssessmentTypeId, a.SubjectId ORDER BY st.Orderby DESC) AS 'RowNumber',
	a.AssessmentTypeId, 
	a.SubjectId, 
	scr.GradeLevel,
	CASE (SELECT Count(*) FROM tblAssessmentScore  WHERE AssessmentId =scr.AssessmentId 
	AND GradeLevel=scr.GradeLevel
	AND studentid in (SELECT CS.studentid FROM TBLCLASSSTUDENT CS 
					  INNER JOIN @tblTempStudent TS ON CS.StudentId = TS.StudentId	
					  WHERE CS.CLASSID IN (SELECT ClassId FROM @tblTempClassID))
	and ScaledScoreProjDif IS NOT NULL) WHEN 0 THEN 0 ELSE 1 END AS Flag,  
	st.Orderby
	FROM
	tblAssessmentScore scr
	JOIN tblAssessment a ON scr.AssessmentId = a.AssessmentId
	JOIN tblSchoolTerm st ON a.SchoolTermId = st.SchoolTermId
	WHERE
	scr.schoolId IN (
		SELECT schoolId 
		FROM tblSchool 
		WHERE DistrictId = @DistrictId
		)
	AND a.SchoolYearId = @SchoolYearId		
	GROUP BY scr.AssessmentId, a.AssessmentTypeId, a.SubjectId, scr.GradeLevel, st.Orderby		 
	
	--Step 1
	INSERT INTO @ChildTemp
	SELECT *
	, ROW_NUMBER() OVER (PARTITION BY BT.AssessmentTypeId, BT.SubjectId,BT.GradeLevel ORDER BY BT.RowNumber) AS 'RowNumberNew'
	FROM @BaseTemp BT  WHERE BT.Flag =1

	--Step 2
	DELETE @BaseTemp 
	FROM @BaseTemp BT
	INNER JOIN @ChildTemp CT ON BT.AssessmentTypeId = CT.AssessmentTypeId and BT.SubjectId = CT.SubjectId and BT.GradeLevel = CT.GradeLevel
	WHERE CT.RowNumberNew = 1
	
	--Step 3
	INSERT INTO @BaseTemp (AssessmentId,RowNumber,AssessmentTypeId,SubjectId,GradeLevel, Flag,Orderby)
	SELECT AssessmentId,RowNumber,AssessmentTypeId,SubjectId,GradeLevel,Flag,Orderby  
	FROM @ChildTemp CT WHERE CT.RowNumberNew = 1

	--Step 4
	Delete from @ChildTemp

	--Step 5
	INSERT INTO @ChildTemp
	SELECT *
	, ROW_NUMBER() OVER (PARTITION BY BT.AssessmentTypeId, BT.SubjectId,BT.GradeLevel ORDER BY BT.RowNumber) AS 'RowNumberNew'
	FROM @BaseTemp BT  WHERE BT.Flag =0

	--step 6
	Delete from @BaseTemp where Flag = 0

	--step 7
	INSERT INTO @BaseTemp (AssessmentId,RowNumber,AssessmentTypeId,SubjectId,GradeLevel, Flag,Orderby)
	SELECT AssessmentId,RowNumber,AssessmentTypeId,SubjectId,GradeLevel,Flag,Orderby  
	FROM @ChildTemp CT WHERE CT.RowNumberNew = 1

	--step 8
	INSERT INTO @DistrictsLastAssessmentsTable(AssessmentId,AssessmentTypeId,SubjectId, GradeLevel)
	SELECT AssessmentId,AssessmentTypeId,SubjectId, GradeLevel FROM @BaseTemp 
RETURN
END



