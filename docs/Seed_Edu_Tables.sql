/*
============================================================================
SEED SCRIPT: University Portal Foundation Data (Edu_*)
DESCRIPTION: Populates university master tables with mock data for development.
============================================================================
*/
SET NOCOUNT ON;
GO
DELETE FROM [Edu_Students];
DELETE FROM [Edu_EmployeePositions];
DELETE FROM [Edu_Employees];
DELETE FROM [Edu_Users];
DELETE FROM [Edu_Specializations_OrgUnits];
DELETE FROM [Edu_SpecialitySpecializations];
DELETE FROM [Edu_Specializations];
DELETE FROM [Edu_Specialities];
DELETE FROM [Edu_OrgUnits];
GO
-- 1. Edu_OrgUnitTypes
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 1) INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (1, N'Department (Kafedra)');
IF NOT EXISTS (SELECT 1 FROM [Edu_OrgUnitTypes] WHERE [ID] = 2) INSERT INTO [Edu_OrgUnitTypes] ([ID], [Title]) VALUES (2, N'Institute');
GO
-- 2. Edu_OrgUnits
INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID]) VALUES (1, NULL, N'Институт автоматики и информационных технологий', 0, N'ИАиИТ', 2);
INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID]) VALUES (101, 1, N'Программная инженерия', 0, N'ПИ', 1);
INSERT INTO [Edu_OrgUnits] ([ID], [ParentID], [Title], [Deleted], [ShortTitle], [TypeID]) VALUES (102, 1, N'Информационная безопасность', 0, N'ИБ', 1);
GO
-- 3. Edu_SpecialityLevels
IF NOT EXISTS (SELECT 1 FROM [Edu_SpecialityLevels] WHERE [ID] = 1) INSERT INTO [Edu_SpecialityLevels] ([ID], [Title]) VALUES (1, N'Bachelor');
GO
-- 4. Edu_Specializations (Направления подготовки)
INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code]) VALUES (1, N'Ақпараттық технологиялар', N'Ақпараттық технологиялар', N'Information Technologies', 'IT-01');
INSERT INTO [Edu_Specializations] ([Id], [TitleRu], [TitleKz], [TitleEn], [Code]) VALUES (2, N'Информационная безопасность', N'Ақпараттық қауіпсіздік', N'Information Security', 'IS-01');
GO
-- 5. Edu_Specialities (Образовательные программы)
INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID]) VALUES (1, '6B06101', N'Computer Science', 4, 0, N'Computer Science', 1);
INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (1, 1, 1);
INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID]) VALUES (2, '6B06102', N'Информационные системы', 4, 0, N'Информационные системы', 1);
INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (2, 2, 1);
INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID]) VALUES (3, '6B06103', N'Информационная безопасность (ИБ)', 4, 0, N'Информационная безопасность (ИБ)', 1);
INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (3, 3, 2);
INSERT INTO [Edu_Specialities] ([ID], [Code], [Title], [YearsOfStudy], [Deleted], [ShortTitle], [LevelID]) VALUES (4, '6B06104', N'Системы информационной безопасности (СИБ)', 4, 0, N'Системы информационной безопасности (СИБ)', 1);
INSERT INTO [Edu_SpecialitySpecializations] ([ID], [SpecialityId], [SpecializationId]) VALUES (4, 4, 2);
GO
-- 6. Edu_Specializations_OrgUnits (Привязка направлений к кафедрам)
INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID]) VALUES (1, 1, 101);
INSERT INTO [Edu_Specializations_OrgUnits] ([ID], [SpecializationID], [OrgUnitID]) VALUES (2, 2, 102);
GO
-- 7. Edu_Positions
IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 1) INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Lectures], [Practices], [Labs]) VALUES (1, N'Professor', 0, 10, 5, 0);
IF NOT EXISTS (SELECT 1 FROM [Edu_Positions] WHERE [ID] = 2) INSERT INTO [Edu_Positions] ([ID], [Title], [Deleted], [Lectures], [Practices], [Labs]) VALUES (2, N'Associate Professor', 0, 8, 8, 0);
GO
-- 8. Edu_StudentStatuses
IF NOT EXISTS (SELECT 1 FROM [Edu_StudentStatuses] WHERE [ID] = 1) INSERT INTO [Edu_StudentStatuses] ([ID], [Title]) VALUES (1, N'Active');
GO
-- 9. Semester and User setup
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (1, N'Ахметов', N'Админ', N'Системный', 'admin@university.edu', '1980-01-01', 1, '800101123456');
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (2, N'Сыздыкова', N'Заведующая', N'Кафедрой', 'head@university.edu', '1975-01-01', 0, '750101123456');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (2, 0, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (1, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 1, 2, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (100, N'Жумабеков', N'Руслан', N'Бауыржанович', 't100@university.edu', '1980-01-01', 1, '800101000100');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (100, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (100, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 100, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (101, N'Омарова', N'Алия', N'Муратовна', 't101@university.edu', '1980-01-01', 0, '800101000101');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (101, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (101, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 101, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (102, N'Нургалиева', N'Гульнур', N'Кайратовна', 't102@university.edu', '1980-01-01', 0, '800101000102');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (102, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (102, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 102, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (103, N'Каримова', N'Жазира', N'Бауыржановна', 't103@university.edu', '1980-01-01', 0, '800101000103');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (103, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (103, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 103, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (104, N'Омарова', N'Зарина', N'Муратовна', 't104@university.edu', '1980-01-01', 0, '800101000104');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (104, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (104, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 104, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (105, N'Омаров', N'Али', N'Бауыржанович', 't105@university.edu', '1980-01-01', 1, '800101000105');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (105, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (105, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 105, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (106, N'Керимбекова', N'Гульнур', N'Бауыржановна', 't106@university.edu', '1980-01-01', 0, '800101000106');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (106, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (106, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 106, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (107, N'Сагитов', N'Тимур', N'Талгатович', 't107@university.edu', '1980-01-01', 1, '800101000107');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (107, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (107, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 107, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (108, N'Керимбекова', N'Алия', N'Талгатовна', 't108@university.edu', '1980-01-01', 0, '800101000108');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (108, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (108, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 108, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (109, N'Керимбекова', N'Айнур', N'Ерлановна', 't109@university.edu', '1980-01-01', 0, '800101000109');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (109, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (109, '2020-09-01', 'SYSTEM', SYSDATETIME(), 101, 2, 109, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (110, N'Сагитов', N'Марат', N'Кайратович', 't110@university.edu', '1980-01-01', 1, '800101000110');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (110, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (110, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 110, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (111, N'Нургалиев', N'Али', N'Талгатович', 't111@university.edu', '1980-01-01', 1, '800101000111');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (111, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (111, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 111, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (112, N'Сыздыкова', N'Асель', N'Муратовна', 't112@university.edu', '1980-01-01', 0, '800101000112');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (112, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (112, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 112, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (113, N'Нургалиев', N'Руслан', N'Талгатович', 't113@university.edu', '1980-01-01', 1, '800101000113');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (113, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (113, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 113, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (114, N'Ахметов', N'Арман', N'Муратович', 't114@university.edu', '1980-01-01', 1, '800101000114');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (114, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (114, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 114, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (115, N'Керимбекова', N'Динара', N'Муратовна', 't115@university.edu', '1980-01-01', 0, '800101000115');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (115, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (115, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 115, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (116, N'Ахметов', N'Ильяс', N'Ерланович', 't116@university.edu', '1980-01-01', 1, '800101000116');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (116, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (116, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 116, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (117, N'Абдрахманов', N'Али', N'Талгатович', 't117@university.edu', '1980-01-01', 1, '800101000117');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (117, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (117, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 117, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (118, N'Оспанов', N'Тимур', N'Бауыржанович', 't118@university.edu', '1980-01-01', 1, '800101000118');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (118, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (118, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 118, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (119, N'Сагитова', N'Мадина', N'Муратовна', 't119@university.edu', '1980-01-01', 0, '800101000119');
INSERT INTO [Edu_Employees] ([ID], [IsAdvisor], [RoleGroupId]) VALUES (119, 1, 1);
INSERT INTO [Edu_EmployeePositions] ([ID], [StartedOn], [LastUpdatedBy], [LastUpdatedOn], [OrgUnitID], [PositionID], [EmployeeID], [IsMainPosition]) VALUES (119, '2020-09-01', 'SYSTEM', SYSDATETIME(), 102, 2, 119, 1);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (200, N'Алиева', N'Асель', N'Талгатовна', 's200@university.edu', '2002-01-01', 0, '020101000200');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (200, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 109);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (201, N'Керимбекова', N'Динара', N'Талгатовна', 's201@university.edu', '2002-01-01', 0, '020101000201');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (201, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 102);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (202, N'Нургалиев', N'Нурлан', N'Ерланович', 's202@university.edu', '2002-01-01', 1, '020101000202');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (202, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 109);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (203, N'Тлеуов', N'Серик', N'Ерланович', 's203@university.edu', '2002-01-01', 1, '020101000203');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (203, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 107);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (204, N'Сагитова', N'Айгерим', N'Талгатовна', 's204@university.edu', '2002-01-01', 0, '020101000204');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (204, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 104);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (205, N'Оспанов', N'Ерлан', N'Кайратович', 's205@university.edu', '2002-01-01', 1, '020101000205');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (205, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 105);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (206, N'Сагитова', N'Алия', N'Муратовна', 's206@university.edu', '2002-01-01', 0, '020101000206');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (206, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 107);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (207, N'Ибрагимова', N'Динара', N'Ерлановна', 's207@university.edu', '2002-01-01', 0, '020101000207');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (207, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 105);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (208, N'Сыздыкова', N'Гульнур', N'Ерлановна', 's208@university.edu', '2002-01-01', 0, '020101000208');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (208, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 103);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (209, N'Ахметов', N'Ерлан', N'Бауыржанович', 's209@university.edu', '2002-01-01', 1, '020101000209');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (209, 1, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 108);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (210, N'Омарова', N'Айнур', N'Муратовна', 's210@university.edu', '2002-01-01', 0, '020101000210');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (210, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 101);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (211, N'Оспанов', N'Серик', N'Ерланович', 's211@university.edu', '2002-01-01', 1, '020101000211');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (211, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 107);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (212, N'Абдрахманов', N'Марат', N'Муратович', 's212@university.edu', '2002-01-01', 1, '020101000212');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (212, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 100);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (213, N'Оспанова', N'Гульнур', N'Муратовна', 's213@university.edu', '2002-01-01', 0, '020101000213');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (213, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 107);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (214, N'Ибрагимов', N'Марат', N'Кайратович', 's214@university.edu', '2002-01-01', 1, '020101000214');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (214, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 100);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (215, N'Сыздыков', N'Серик', N'Бауыржанович', 's215@university.edu', '2002-01-01', 1, '020101000215');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (215, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 103);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (216, N'Тлеуов', N'Серик', N'Талгатович', 's216@university.edu', '2002-01-01', 1, '020101000216');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (216, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 104);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (217, N'Абдрахманов', N'Азамат', N'Муратович', 's217@university.edu', '2002-01-01', 1, '020101000217');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (217, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 109);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (218, N'Ахметова', N'Асель', N'Ерлановна', 's218@university.edu', '2002-01-01', 0, '020101000218');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (218, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 102);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (219, N'Оспанов', N'Али', N'Кайратович', 's219@university.edu', '2002-01-01', 1, '020101000219');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (219, 2, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 101);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (220, N'Омаров', N'Али', N'Талгатович', 's220@university.edu', '2002-01-01', 1, '020101000220');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (220, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 110);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (221, N'Сагитова', N'Сауле', N'Бауыржановна', 's221@university.edu', '2002-01-01', 0, '020101000221');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (221, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 114);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (222, N'Алиев', N'Дамир', N'Муратович', 's222@university.edu', '2002-01-01', 1, '020101000222');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (222, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 113);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (223, N'Омарова', N'Динара', N'Муратовна', 's223@university.edu', '2002-01-01', 0, '020101000223');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (223, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 111);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (224, N'Сагитова', N'Амина', N'Ерлановна', 's224@university.edu', '2002-01-01', 0, '020101000224');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (224, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 117);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (225, N'Оспанова', N'Алия', N'Бауыржановна', 's225@university.edu', '2002-01-01', 0, '020101000225');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (225, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 114);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (226, N'Керимбеков', N'Арман', N'Талгатович', 's226@university.edu', '2002-01-01', 1, '020101000226');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (226, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 118);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (227, N'Тлеуов', N'Ильяс', N'Бауыржанович', 's227@university.edu', '2002-01-01', 1, '020101000227');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (227, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 113);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (228, N'Омаров', N'Дамир', N'Муратович', 's228@university.edu', '2002-01-01', 1, '020101000228');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (228, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 113);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (229, N'Оспанова', N'Амина', N'Талгатовна', 's229@university.edu', '2002-01-01', 0, '020101000229');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (229, 3, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 110);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (230, N'Жумабекова', N'Алия', N'Талгатовна', 's230@university.edu', '2002-01-01', 0, '020101000230');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (230, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 116);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (231, N'Нургалиев', N'Арман', N'Кайратович', 's231@university.edu', '2002-01-01', 1, '020101000231');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (231, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 119);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (232, N'Каримова', N'Жазира', N'Ерлановна', 's232@university.edu', '2002-01-01', 0, '020101000232');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (232, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 113);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (233, N'Алиева', N'Айнур', N'Кайратовна', 's233@university.edu', '2002-01-01', 0, '020101000233');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (233, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 110);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (234, N'Ибрагимова', N'Мадина', N'Муратовна', 's234@university.edu', '2002-01-01', 0, '020101000234');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (234, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 116);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (235, N'Ахметова', N'Алия', N'Муратовна', 's235@university.edu', '2002-01-01', 0, '020101000235');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (235, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 114);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (236, N'Сагитова', N'Сауле', N'Ерлановна', 's236@university.edu', '2002-01-01', 0, '020101000236');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (236, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 111);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (237, N'Каримов', N'Арман', N'Ерланович', 's237@university.edu', '2002-01-01', 1, '020101000237');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (237, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 117);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (238, N'Нургалиева', N'Асель', N'Бауыржановна', 's238@university.edu', '2002-01-01', 0, '020101000238');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (238, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 115);
INSERT INTO [Edu_Users] ([ID], [LastName], [FirstName], [MiddleName], [Email], [DOB], [Male], [IIN]) VALUES (239, N'Сагитова', N'Амина', N'Муратовна', 's239@university.edu', '2002-01-01', 0, '020101000239');
INSERT INTO [Edu_Students] ([StudentID], [SpecialityID], [StatusID], [Year], [NeedsDorm], [AltynBelgi], [GPA], [LastUpdatedBy], [LastUpdatedOn], [AdvisorID]) VALUES (239, 4, 1, 4, 0, 0, 3.5, 'SYSTEM', SYSDATETIME(), 117);
GO

-- 10. Edu_SemesterTypes
IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 1) INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (1, N'Autumn', 1);
IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 2) INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (2, N'Spring', 2);
IF NOT EXISTS (SELECT 1 FROM [Edu_SemesterTypes] WHERE [ID] = 3) INSERT INTO [Edu_SemesterTypes] ([ID], [Title], [OrderBy]) VALUES (3, N'Summer', 3);
GO

-- 11. Edu_Semesters
INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID]) VALUES (1, N'Autumn 2025-2026', '2025-09-01', '2026-01-31', 2025, 1);
INSERT INTO [Edu_Semesters] ([ID], [Title], [StartsOn], [EndsOn], [StudyYear], [SemesterTypeID]) VALUES (2, N'Spring 2025-2026', '2026-02-01', '2026-06-30', 2025, 2);
GO
