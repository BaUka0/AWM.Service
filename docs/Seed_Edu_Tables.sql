/*
============================================================================
SEED SCRIPT: University Portal Foundation Data (Edu_*)
DESCRIPTION: Populates university master tables with mock data for development.
ORDER: Follows Foreign Key constraints.
============================================================================
*/

SET NOCOUNT ON;
GO

PRINT 'Seeding [Edu_*] tables...';

-- 1. Edu_OrgUnitTypes
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 1)
    INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (1, N'Department (Kafedra)');
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 2)
    INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (2, N'Institute');
GO

-- 2. Edu_OrgUnits
-- Institute (Parent)
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 1)
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (1, NULL, N'Institute of Information Technology', 0, N'IIT', 2);

-- Departments (Children)
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 101)
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (101, 1, N'Software Engineering Department', 0, N'SE', 1);

IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnits] WHERE [ID] = 102)
    INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID])
    VALUES (102, 1, N'Computer Science Department', 0, N'CS', 1);
GO

-- 3. Edu_SpecialityLevels
IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 1)
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title]) VALUES (1, N'Bachelor');
IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 2)
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title]) VALUES (2, N'Master');
IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 3)
    INSERT INTO [Edu_SpecialityLevels] ([ID], [Title]) VALUES (3, N'PhD');
GO

-- 4. Edu_Specialities
IF NOT EXISTS (SELECT 1 FROM [Edu_Specialities] WHERE [ID] = 1)
    INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID])
    VALUES (1, '6B06101', N'Software Engineering', 4, 0, N'SE', 1);

IF NOT EXISTS (SELECT 1 FROM [Edu_Specialities] WHERE [ID] = 2)
    INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID])
    VALUES (2, '6B06102', N'Computer Science', 4, 0, N'CS', 1);
GO

-- 5. Edu_Specializations
IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations] WHERE [Id] = 1)
    INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code])
    VALUES (1, N'Web Development', N'Web Development KZ', N'Web Development', 'WD-01');
GO

-- 6. Edu_SpecialitySpecializations
IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialitySpecializations] WHERE [ID] = 1)
    INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId])
    VALUES (1, 1, 1);
GO

-- 7. Edu_Specializations_OrgUnits
IF NOT EXISTS (SELECT 1 FROM [Edu_Specializations_OrgUnits] WHERE [ID] = 1)
    INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID])
    VALUES (1, 1, 101);
GO

-- 8. Edu_StudentStatuses
IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 1)
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title]) VALUES (1, N'Active');
IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 2)
    INSERT INTO [Edu_StudentStatuses] ([ID], [Title]) VALUES (2, N'Graduated');
GO

-- 9. Edu_Users
-- System Admin
IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 1)
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN])
    VALUES (1, N'Admin', N'System', NULL, 'admin@university.edu', '1980-01-01', 1, '+77010000001', '800101123456');

-- Employees (Professors)
IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 2)
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN])
    VALUES (2, N'Ivanov', N'Ivan', N'Ivanovich', 'i.ivanov@university.edu', '1975-05-15', 1, '+77010000002', '750515123456');

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 3)
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN])
    VALUES (3, N'Sidorov', N'Petr', N'Petrovich', 'p.sidorov@university.edu', '1982-10-20', 1, '+77010000003', '821020123456');

-- Students
IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 10)
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN])
    VALUES (10, N'Studentov', N'Arman', NULL, 'a.studentov@university.edu', '2002-03-12', 1, '+77010000010', '020312123456');

IF NOT EXISTS (SELECT 1 FROM [Edu_Users] WHERE [ID] = 11)
    INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [MobilePhone], [IIN])
    VALUES (11, N'Amanova', N'Aigerim', NULL, 'a.amanova@university.edu', '2002-08-25', 0, '+77010000011', '020825623456');
GO

-- 10. Edu_Employees
IF NOT EXISTS (SELECT 1 FROM [Edu_Employees] WHERE [ID] = 2)
    INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (2, 1, 1);
IF NOT EXISTS (SELECT 1 FROM [Edu_Employees] WHERE [ID] = 3)
    INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (3, 1, 1);
GO

-- 11. Edu_Positions
IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 1)
    INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Lectures], [Practices], [Labs])
    VALUES (1, N'Professor', 0, 10, 5, 0);
IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 2)
    INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Lectures], [Practices], [Labs])
    VALUES (2, N'Associate Professor', 0, 8, 8, 0);
GO

-- 12. Edu_EmployeePositions
IF NOT EXISTS (SELECT 1 FROM [Edu_EmployeePositions] WHERE [ID] = 1)
    INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition])
    VALUES (1, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 1, 2, 1);

IF NOT EXISTS (SELECT 1 FROM [Edu_EmployeePositions] WHERE [ID] = 2)
    INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition])
    VALUES (2, '2021-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 3, 1);
GO

-- 13. Edu_SemesterTypes
IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 1)
    INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (1, N'Fall', 1);
IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 2)
    INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (2, N'Spring', 2);
GO

-- 14. Edu_Semesters
IF NOT EXISTS (SELECT 1 FROM [Edu_Semesters] WHERE [ID] = 1)
    INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID])
    VALUES (1, N'Fall 2023-2024', '2023-09-01', '2024-01-20', 2023, 1);

IF NOT EXISTS (SELECT 1 FROM [Edu_Semesters] WHERE [ID] = 2)
    INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID])
    VALUES (2, N'Spring 2023-2024', '2024-01-25', '2024-06-30', 2023, 2);
GO

-- 15. Edu_Students
IF NOT EXISTS (SELECT 1 FROM [Edu_Students] WHERE [StudentID] = 10)
    INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID])
    VALUES (10, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 2);

IF NOT EXISTS (SELECT 1 FROM [Edu_Students] WHERE [StudentID] = 11)
    INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID])
    VALUES (11, 2, 1, 4, 0, 0, 3.8, 'SYSTEM', SYSDATETIME(), 3);
GO

PRINT '[Edu_*] tables seeded successfully.';
GO
