SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (1, N'Кафедра');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (2, N'Институт');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (3, N'Университет');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (1, NULL, N'AWM University', 0, N'AWM', 3);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 10)
BEGIN
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (10, 1, N'Institute of Digital Technologies', 0, N'IDT', 2);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 100)
BEGIN
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (100, 10, N'Department of Software Engineering', 0, N'SE', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 101)
BEGIN
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (101, 10, N'Department of Computer Science', 0, N'CS', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title], [NoBDID]) VALUES (1, N'Bachelor', N'01');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title], [NoBDID]) VALUES (2, N'Master', N'02');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title], [NoBDID]) VALUES (3, N'PhD', N'03');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specialities] WHERE [ID] = 10)
BEGIN
    INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID])
    VALUES (10, N'6B06101', N'Software Engineering', 4, 0, N'SE', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specialities] WHERE [ID] = 11)
BEGIN
    INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID])
    VALUES (11, N'6B06102', N'Computer Science', 4, 0, N'CS', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specialities] WHERE [ID] = 20)
BEGIN
    INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID])
    VALUES (20, N'7M06101', N'Data Science', 2, 0, N'DS', 2);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations] WHERE [Id] = 1001)
BEGIN
    INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code])
    VALUES (1001, N'Программная инженерия', N'Бағдарламалық инженерия', N'Software Engineering', N'SE');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations] WHERE [Id] = 1002)
BEGIN
    INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code])
    VALUES (1002, N'Искусственный интеллект', N'Жасанды интеллект', N'Artificial Intelligence', N'AI');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations] WHERE [Id] = 1003)
BEGIN
    INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code])
    VALUES (1003, N'Наука о данных', N'Деректер ғылымы', N'Data Science', N'DS');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialitySpecializations] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (1, 10, 1001);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialitySpecializations] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (2, 11, 1002);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialitySpecializations] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (3, 20, 1003);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations_OrgUnits] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID]) VALUES (1, 1001, 100);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations_OrgUnits] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID]) VALUES (2, 1002, 101);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations_OrgUnits] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID]) VALUES (3, 1003, 101);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title], [NOBDID]) VALUES (1, N'Active', N'01');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title], [NOBDID]) VALUES (2, N'Academic leave', N'02');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title], [NOBDID]) VALUES (3, N'Graduated', N'03');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 4)
BEGIN
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title], [NOBDID]) VALUES (4, N'Expelled', N'04');
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (1, N'System', N'User', NULL, N'system@awm.local', NULL, NULL, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 100)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (100, N'Admin', N'User', NULL, N'admin@awm.local', '1990-01-01', 1, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 101)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (101, N'Ivanov', N'Ivan', N'Ivanovich', N'supervisor@awm.local', '1985-03-15', 1, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 102)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (102, N'Petrova', N'Anna', N'Sergeevna', N'reviewer@awm.local', '1988-07-20', 0, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 200)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (200, N'Sidorov', N'Petr', N'Alexandrovich', N'student1@awm.local', '2004-11-02', 1, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 201)
BEGIN
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN], [PhotoFileName], [PhotoFileData])
    VALUES (201, N'Kim', N'Aigerim', N'Nurlanovna', N'student2@awm.local', '2005-05-12', 0, NULL, NULL, NULL, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Employees] WHERE [ID] = 100)
BEGIN
    INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (100, 0, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Employees] WHERE [ID] = 101)
BEGIN
    INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (101, 1, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Employees] WHERE [ID] = 102)
BEGIN
    INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (102, 0, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Description], [Lectures], [Practices], [Labs], [CategoryID])
    VALUES (1, N'Administrator', 0, NULL, 0, 0, 0, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Description], [Lectures], [Practices], [Labs], [CategoryID])
    VALUES (2, N'Professor', 0, NULL, 0, 0, 0, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Description], [Lectures], [Practices], [Labs], [CategoryID])
    VALUES (3, N'Senior Lecturer', 0, NULL, 0, 0, 0, NULL);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_EmployeePositions] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [EndedOn], [LastUpdatedBy], [LastUpdatedOn], [Rate], [IsMainPosition], [HrOrderId], [OrgUnitID], [PositionID], [EmployeeID])
    VALUES (1, '2024-09-01', NULL, N'seed', SYSUTCDATETIME(), 1, 1, NULL, 1, 1, 100);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_EmployeePositions] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [EndedOn], [LastUpdatedBy], [LastUpdatedOn], [Rate], [IsMainPosition], [HrOrderId], [OrgUnitID], [PositionID], [EmployeeID])
    VALUES (2, '2024-09-01', NULL, N'seed', SYSUTCDATETIME(), 1, 1, NULL, 100, 2, 101);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_EmployeePositions] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [EndedOn], [LastUpdatedBy], [LastUpdatedOn], [Rate], [IsMainPosition], [HrOrderId], [OrgUnitID], [PositionID], [EmployeeID])
    VALUES (3, '2024-09-01', NULL, N'seed', SYSUTCDATETIME(), 1, 1, NULL, 101, 3, 102);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 1)
BEGIN
    INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (1, N'Fall', 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 2)
BEGIN
    INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (2, N'Spring', 2);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 3)
BEGIN
    INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (3, N'Summer', 3);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Semesters] WHERE [ID] = 202501)
BEGIN
    INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID])
    VALUES (202501, N'2025-2026 Fall', '2025-09-01T00:00:00', '2026-01-15T23:59:59', 2025, 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Semesters] WHERE [ID] = 202502)
BEGIN
    INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID])
    VALUES (202502, N'2025-2026 Spring', '2026-01-16T00:00:00', '2026-06-30T23:59:59', 2025, 2);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Semesters] WHERE [ID] = 202601)
BEGIN
    INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID])
    VALUES (202601, N'2026-2027 Fall', '2026-09-01T00:00:00', '2027-01-15T23:59:59', 2026, 1);
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Students] WHERE [StudentID] = 200)
BEGIN
    INSERT INTO [Edu_Students] (
        [StudentID], [SpecialityID], [StatusID], [CategoryID], [NeedsDorm], [AltynBelgi], [Year], [RupID], [EntryDate], [GPA],
        [LastUpdatedBy], [LastUpdatedOn], [GraduatedOn], [AcademicStatusEndsOn], [AcademicStatusStartsOn], [GPA_Y],
        [IsPersonalDataComplete], [HosterPrivelegeID], [MinorSpecialityID], [EnrollmentTypeId], [EctsGPA], [EctsGPA_Y],
        [IsScholarship], [ScholarshipTypeID], [ScholarshipOrderNumber], [ScholarshipOrderDate], [ScholarshipDateStart],
        [ScholarshipDateEnd], [FundingID], [IsKNB], [EducationTypeID], [EducationPaymentTypeID], [GrantTypeID],
        [EducationDurationID], [StudyLanguageID], [AcademicStatusID], [AdvisorID]
    )
    VALUES (
        200, 10, 1, NULL, 0, 0, 4, NULL, '2022-09-01', 3.45,
        N'seed', SYSUTCDATETIME(), NULL, NULL, NULL, NULL,
        1, NULL, NULL, NULL, NULL, NULL,
        0, NULL, NULL, NULL, NULL,
        NULL, NULL, 0, NULL, NULL, NULL,
        NULL, NULL, NULL, 101
    );
END;

IF NOT EXISTS (SELECT 1 FROM [Edu_Students] WHERE [StudentID] = 201)
BEGIN
    INSERT INTO [Edu_Students] (
        [StudentID], [SpecialityID], [StatusID], [CategoryID], [NeedsDorm], [AltynBelgi], [Year], [RupID], [EntryDate], [GPA],
        [LastUpdatedBy], [LastUpdatedOn], [GraduatedOn], [AcademicStatusEndsOn], [AcademicStatusStartsOn], [GPA_Y],
        [IsPersonalDataComplete], [HosterPrivelegeID], [MinorSpecialityID], [EnrollmentTypeId], [EctsGPA], [EctsGPA_Y],
        [IsScholarship], [ScholarshipTypeID], [ScholarshipOrderNumber], [ScholarshipOrderDate], [ScholarshipDateStart],
        [ScholarshipDateEnd], [FundingID], [IsKNB], [EducationTypeID], [EducationPaymentTypeID], [GrantTypeID],
        [EducationDurationID], [StudyLanguageID], [AcademicStatusID], [AdvisorID]
    )
    VALUES (
        201, 11, 1, NULL, 0, 0, 4, NULL, '2022-09-01', 3.72,
        N'seed', SYSUTCDATETIME(), NULL, NULL, NULL, NULL,
        1, NULL, NULL, NULL, NULL, NULL,
        1, NULL, N'SCH-2025-001', '2025-09-01', '2025-09-01',
        '2026-06-30', NULL, 0, NULL, NULL, NULL,
        NULL, NULL, NULL, 101
    );
END;

COMMIT TRANSACTION;

PRINT 'Edu_* seed data has been applied successfully.';
