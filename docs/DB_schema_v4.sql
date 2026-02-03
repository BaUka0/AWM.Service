/*
============================================================================
PROJECT: ETMS (Enterprise Thesis Management System)
VERSION: 4.0 (Production Ready - Full Workflow Support)
DBMS: Microsoft SQL Server 2016+
AUTHOR: Senior DB Architect
DATE: 2026-01-24

FEATURES:
- Multi-tenancy (University Root)
- Context-Aware RBAC
- State Machine Workflow (Extended for Directions)
- System-Versioned Temporal Tables (Auto-Audit)
- Support for Team Works (2-5 participants)
- Pre-defense versioning (1, 2, 3)
- Complete Quality Check Cycle (NormControl, Software, AntiPlagiarism, Review)
- Temporal Periods Control
- Notification System

MAJOR CHANGES FROM v3:
+ Directions (Research Areas) management
+ Topic Application mechanism
+ Team Work support (M:M Students-Works)
+ Pre-defense attempts tracking
+ Enhanced Quality Checks with retry cycle
+ External Reviewers & Supervisor Reviews
+ Temporal Periods for workflow stages
+ Notification system
+ Course Works support
============================================================================
*/

-- 1. Создание схем (Namespaces)
GO
CREATE SCHEMA [Org];        -- Организационная структура
GO
CREATE SCHEMA [Common];     -- Общие справочники
GO
CREATE SCHEMA [Auth];       -- Безопасность и Роли
GO
CREATE SCHEMA [Edu];        -- Учебные профили
GO
CREATE SCHEMA [Wf];         -- Workflow Engine
GO
CREATE SCHEMA [Thesis];     -- Дипломные работы
GO
CREATE SCHEMA [Defense];    -- Защита и Оценки
GO

-- =============================================
-- [Org] ОРГАНИЗАЦИОННАЯ СТРУКТУРА
-- =============================================

-- Корневой тенант (Университет)
CREATE TABLE [Org].[Universities] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(255) NOT NULL,
    [Code] NVARCHAR(50) NOT NULL, -- Уникальный код тенанта (напр. 'KAZNU', 'POLYTECH')
    [CreatedAt] DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT [UQ_University_Code] UNIQUE ([Code])
);

CREATE TABLE [Org].[Institutes] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UniversityId] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    CONSTRAINT [FK_Institutes_University] FOREIGN KEY ([UniversityId]) REFERENCES [Org].[Universities]([Id])
);

CREATE TABLE [Org].[Departments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [InstituteId] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Code] NVARCHAR(50),
    CONSTRAINT [FK_Departments_Institute] FOREIGN KEY ([InstituteId]) REFERENCES [Org].[Institutes]([Id])
);

-- =============================================
-- [Common] ОБЩИЕ СПРАВОЧНИКИ
-- =============================================

CREATE TABLE [Common].[AcademicYears] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UniversityId] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL, -- '2025-2026'
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [IsCurrent] BIT DEFAULT 0,
    [IsArchived] BIT DEFAULT 0, -- Read-only режим
    CONSTRAINT [FK_AcademicYears_University] FOREIGN KEY ([UniversityId]) REFERENCES [Org].[Universities]([Id])
);

-- NEW v4: Периоды для этапов workflow
CREATE TABLE [Common].[Periods] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [DepartmentId] INT NOT NULL,
    [AcademicYearId] INT NOT NULL,
    [WorkflowStage] NVARCHAR(100) NOT NULL, 
    -- 'DirectionSubmission', 'TopicCreation', 'TopicSelection', 
    -- 'PreDefense1', 'PreDefense2', 'PreDefense3', 'FinalDefense'
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [IsActive] BIT DEFAULT 1,
    [CreatedAt] DATETIME2 DEFAULT SYSDATETIME(),
    [CreatedBy] INT NOT NULL,
    
    CONSTRAINT [FK_Periods_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Periods_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id]),
    CONSTRAINT [Check_Period_Dates] CHECK ([EndDate] > [StartDate])
);

-- NEW v4: Шаблоны уведомлений
CREATE TABLE [Common].[NotificationTemplates] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [EventType] NVARCHAR(100) NOT NULL, -- 'DirectionApproved', 'ApplicationReceived', 'DefenseScheduled'
    [TitleRu] NVARCHAR(255),
    [TitleKz] NVARCHAR(255),
    [TitleEn] NVARCHAR(255),
    [BodyTemplateRu] NVARCHAR(MAX),
    [BodyTemplateKz] NVARCHAR(MAX),
    [BodyTemplateEn] NVARCHAR(MAX),
    CONSTRAINT [UQ_Template_Event] UNIQUE ([EventType])
);

-- NEW v4: Уведомления пользователей
CREATE TABLE [Common].[Notifications] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [TemplateId] INT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Body] NVARCHAR(MAX),
    [RelatedEntityType] NVARCHAR(50), -- 'Direction', 'Topic', 'Work', 'Application'
    [RelatedEntityId] BIGINT,
    [IsRead] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT SYSDATETIME(),
    
    CONSTRAINT [FK_Notif_Template] FOREIGN KEY ([TemplateId]) REFERENCES [Common].[NotificationTemplates]([Id])
);

-- =============================================
-- [Auth] БЕЗОПАСНОСТЬ (RBAC)
-- =============================================

CREATE TABLE [Auth].[Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UniversityId] INT NOT NULL,
    [Login] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [PasswordHash] NVARCHAR(MAX), 
    [ExternalId] NVARCHAR(255), -- Для SSO (AD/Azure ID)
    [IsActive] BIT DEFAULT 1,
    CONSTRAINT [FK_Users_University] FOREIGN KEY ([UniversityId]) REFERENCES [Org].[Universities]([Id]),
    CONSTRAINT [UQ_User_Email_Tenant] UNIQUE ([UniversityId], [Email])
);

CREATE TABLE [Auth].[Roles] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SystemName] NVARCHAR(50) NOT NULL, -- 'Student', 'Supervisor', 'Admin', 'Expert', 'HeadOfDepartment'
    [DisplayName] NVARCHAR(100),
    [ScopeLevel] NVARCHAR(20) NOT NULL, -- 'Global', 'University', 'Department', 'Commission'
    CONSTRAINT [UQ_Role_SystemName] UNIQUE ([SystemName])
);

-- Матрица доступа (Контекстная)
CREATE TABLE [Auth].[UserRoleAssignments] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    
    -- Контексты
    [DepartmentId] INT NULL, 
    [InstituteId] INT NULL,
    [AcademicYearId] INT NULL,
    
    [ValidFrom] DATETIME2 DEFAULT SYSDATETIME(),
    [ValidTo] DATETIME2 NULL,
    [AssignedBy] INT NULL,
    
    CONSTRAINT [FK_URA_User] FOREIGN KEY ([UserId]) REFERENCES [Auth].[Users]([Id]),
    CONSTRAINT [FK_URA_Role] FOREIGN KEY ([RoleId]) REFERENCES [Auth].[Roles]([Id]),
    CONSTRAINT [FK_URA_Department] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_URA_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id])
);

-- =============================================
-- [Edu] ОБРАЗОВАТЕЛЬНЫЕ ПРОФИЛИ
-- =============================================

CREATE TABLE [Edu].[DegreeLevels] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL, -- 'Bachelor', 'Master', 'PhD'
    [DurationYears] INT NOT NULL
);

CREATE TABLE [Edu].[AcademicPrograms] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [DepartmentId] INT NOT NULL,
    [DegreeLevelId] INT NOT NULL,
    [Code] NVARCHAR(50),
    [Name] NVARCHAR(255),
    CONSTRAINT [FK_Programs_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Programs_Level] FOREIGN KEY ([DegreeLevelId]) REFERENCES [Edu].[DegreeLevels]([Id])
);

CREATE TABLE [Edu].[Students] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [ProgramId] INT NOT NULL,
    [AdmissionYear] INT NOT NULL,
    [CurrentCourse] INT NOT NULL,
    [GroupCode] NVARCHAR(50),
    [Status] NVARCHAR(50) NOT NULL, -- 'Active', 'Graduated', 'Expelled'
    CONSTRAINT [FK_Students_User] FOREIGN KEY ([UserId]) REFERENCES [Auth].[Users]([Id]),
    CONSTRAINT [FK_Students_Program] FOREIGN KEY ([ProgramId]) REFERENCES [Edu].[AcademicPrograms]([Id])
);

CREATE TABLE [Edu].[Staff] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [DepartmentId] INT NOT NULL,
    [Position] NVARCHAR(100),
    [AcademicDegree] NVARCHAR(100), -- 'PhD', 'Master', 'Professor'
    [MaxStudentsLoad] INT DEFAULT 5,
    CONSTRAINT [FK_Staff_User] FOREIGN KEY ([UserId]) REFERENCES [Auth].[Users]([Id]),
    CONSTRAINT [FK_Staff_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id])
);

-- =============================================
-- [Wf] WORKFLOW ENGINE
-- =============================================

CREATE TABLE [Wf].[WorkTypes] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL, -- 'CourseWork', 'DiplomaWork', 'MasterThesis', 'PhD'
    [DegreeLevelId] INT NULL,
    CONSTRAINT [FK_WorkTypes_Level] FOREIGN KEY ([DegreeLevelId]) REFERENCES [Edu].[DegreeLevels]([Id])
);

CREATE TABLE [Wf].[States] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [WorkTypeId] INT NOT NULL,
    [SystemName] NVARCHAR(100) NOT NULL,
    [DisplayName] NVARCHAR(100),
    [IsFinal] BIT DEFAULT 0,
    CONSTRAINT [FK_States_WorkType] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id])
);

CREATE TABLE [Wf].[Transitions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FromStateId] INT NOT NULL,
    [ToStateId] INT NOT NULL,
    [AllowedRoleId] INT NULL,
    [IsAutomatic] BIT DEFAULT 0,
    CONSTRAINT [FK_Trans_From] FOREIGN KEY ([FromStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Trans_To] FOREIGN KEY ([ToStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Trans_Role] FOREIGN KEY ([AllowedRoleId]) REFERENCES [Auth].[Roles]([Id])
);

-- =============================================
-- [Thesis] РАБОТЫ (С TEMPORAL TABLES)
-- =============================================

-- NEW v4: Направления дипломных работ
CREATE TABLE [Thesis].[Directions] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [DepartmentId] INT NOT NULL,
    [SupervisorId] INT NOT NULL,
    [AcademicYearId] INT NOT NULL,
    [WorkTypeId] INT NOT NULL,
    
    [TitleRu] NVARCHAR(500) NOT NULL,
    [TitleEn] NVARCHAR(500),
    [TitleKz] NVARCHAR(500),
    [Description] NVARCHAR(MAX),
    
    [CurrentStateId] INT NOT NULL, -- DirectionDraft, DirectionSubmitted, DirectionApproved, etc.
    [SubmittedAt] DATETIME2,
    [ReviewedAt] DATETIME2,
    [ReviewedBy] INT NULL, -- User who approved/rejected
    [ReviewComment] NVARCHAR(MAX),
    
    -- Системное версионирование
    [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSDATETIME(),
    [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CAST('9999-12-31 23:59:59.9999999' AS DATETIME2),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),
    
    CONSTRAINT [FK_Directions_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Directions_Sup] FOREIGN KEY ([SupervisorId]) REFERENCES [Edu].[Staff]([Id]),
    CONSTRAINT [FK_Directions_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id]),
    CONSTRAINT [FK_Directions_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [FK_Directions_State] FOREIGN KEY ([CurrentStateId]) REFERENCES [Wf].[States]([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Thesis].[DirectionsHistory]));

-- MODIFIED v4: Темы (с поддержкой истории изменений)
CREATE TABLE [Thesis].[Topics] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [DirectionId] BIGINT NULL, -- NEW v4: Связь с направлением
    [AcademicYearId] INT NOT NULL,
    [DepartmentId] INT NOT NULL,
    [SupervisorId] INT NOT NULL,
    [WorkTypeId] INT NOT NULL,
    
    [TitleRu] NVARCHAR(500) NOT NULL,
    [TitleEn] NVARCHAR(500),
    [TitleKz] NVARCHAR(500), -- NEW v4
    [Description] NVARCHAR(MAX),
    [MaxParticipants] INT DEFAULT 1, -- NEW v4: 1-5 participants
    [IsApproved] BIT DEFAULT 0,
    
    -- Системное версионирование
    [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSDATETIME(),
    [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CAST('9999-12-31 23:59:59.9999999' AS DATETIME2),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [FK_Topics_Direction] FOREIGN KEY ([DirectionId]) REFERENCES [Thesis].[Directions]([Id]),
    CONSTRAINT [FK_Topics_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id]),
    CONSTRAINT [FK_Topics_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Topics_Sup] FOREIGN KEY ([SupervisorId]) REFERENCES [Edu].[Staff]([Id]),
    CONSTRAINT [FK_Topics_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [Check_Participants_Positive] CHECK ([MaxParticipants] BETWEEN 1 AND 5)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Thesis].[TopicsHistory]));

-- NEW v4: Заявки студентов на темы
CREATE TABLE [Thesis].[TopicApplications] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TopicId] BIGINT NOT NULL,
    [StudentId] INT NOT NULL,
    [MotivationLetter] NVARCHAR(MAX),
    [AppliedAt] DATETIME2 DEFAULT SYSDATETIME(),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Submitted', -- Submitted, Accepted, Rejected
    [ReviewedAt] DATETIME2,
    [ReviewedBy] INT NULL,
    [ReviewComment] NVARCHAR(MAX),
    
    CONSTRAINT [FK_Applications_Topic] FOREIGN KEY ([TopicId]) REFERENCES [Thesis].[Topics]([Id]),
    CONSTRAINT [FK_Applications_Student] FOREIGN KEY ([StudentId]) REFERENCES [Edu].[Students]([Id]),
    CONSTRAINT [Check_App_Status] CHECK ([Status] IN ('Submitted', 'Accepted', 'Rejected'))
);

-- MODIFIED v4: Работы студентов (Основная таблица с версионированием)
CREATE TABLE [Thesis].[StudentWorks] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TopicId] BIGINT NULL,
    [AcademicYearId] INT NOT NULL,
    [DepartmentId] INT NOT NULL, -- NEW v4: Denormalized for performance
    [CurrentStateId] INT NOT NULL,
    
    [FinalGrade] NVARCHAR(10),
    [IsDefended] BIT DEFAULT 0,
    [CreatedBy] INT NOT NULL, 
    [LastModifiedBy] INT NOT NULL,

    -- Системное версионирование
    [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSDATETIME(),
    [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CAST('9999-12-31 23:59:59.9999999' AS DATETIME2),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [FK_Works_Topic] FOREIGN KEY ([TopicId]) REFERENCES [Thesis].[Topics]([Id]),
    CONSTRAINT [FK_Works_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id]),
    CONSTRAINT [FK_Works_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Works_State] FOREIGN KEY ([CurrentStateId]) REFERENCES [Wf].[States]([Id])
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Thesis].[StudentWorksHistory]));

-- NEW v4: Участники работы (M:M для командных работ)
CREATE TABLE [Thesis].[WorkParticipants] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [StudentId] INT NOT NULL,
    [Role] NVARCHAR(50) DEFAULT 'Member', -- 'Leader', 'Member'
    [JoinedAt] DATETIME2 DEFAULT SYSDATETIME(),
    
    CONSTRAINT [FK_Participants_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Participants_Student] FOREIGN KEY ([StudentId]) REFERENCES [Edu].[Students]([Id]),
    CONSTRAINT [UQ_Work_Student] UNIQUE ([WorkId], [StudentId])
);

-- История воркфлоу (бизнес-лог)
CREATE TABLE [Thesis].[WorkflowHistory] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [FromStateId] INT NULL,
    [ToStateId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Comment] NVARCHAR(MAX),
    [TransitionDate] DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT [FK_WfHist_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id])
);

-- Вложения (без BLOB)
CREATE TABLE [Thesis].[Attachments] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [StateId] INT NULL, -- На каком этапе загружено
    [AttachmentType] NVARCHAR(50) NOT NULL, -- 'Draft', 'Final', 'Presentation', 'Software'
    [FileName] NVARCHAR(255) NOT NULL,
    [FileStoragePath] NVARCHAR(1000) NOT NULL, -- MODIFIED v4: NVARCHAR(MAX) -> NVARCHAR(1000)
    [FileHash] CHAR(64) NOT NULL, -- SHA256
    [UploadedBy] INT NOT NULL,
    [UploadedAt] DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT [FK_Attach_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id])
);

-- NEW v4: Эксперты по проверкам
CREATE TABLE [Thesis].[Experts] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [DepartmentId] INT NOT NULL,
    [ExpertiseType] NVARCHAR(50) NOT NULL, -- 'NormControl', 'SoftwareCheck', 'AntiPlagiarism'
    [IsActive] BIT DEFAULT 1,
    
    CONSTRAINT [FK_Expert_User] FOREIGN KEY ([UserId]) REFERENCES [Auth].[Users]([Id]),
    CONSTRAINT [FK_Expert_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [Check_ExpertType] CHECK ([ExpertiseType] IN ('NormControl', 'SoftwareCheck', 'AntiPlagiarism'))
);

-- MODIFIED v4: Проверки качества (расширенная)
CREATE TABLE [Thesis].[QualityChecks] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [CheckType] NVARCHAR(50) NOT NULL, -- 'NormControl', 'SoftwareCheck', 'AntiPlagiarism'
    [AssignedExpertId] INT NULL, -- NEW v4: Renamed from ExpertId
    [AttemptNumber] INT DEFAULT 1, -- NEW v4: Track retry attempts
    [IsPassed] BIT NOT NULL,
    [ResultValue] DECIMAL(5,2), -- 85.5% для антиплагиата
    [Comment] NVARCHAR(MAX),
    [DocumentPath] NVARCHAR(1000), -- NEW v4: Signed document for AntiPlagiarism
    [CheckedAt] DATETIME2 DEFAULT SYSDATETIME(),
    CONSTRAINT [FK_Check_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Check_Expert] FOREIGN KEY ([AssignedExpertId]) REFERENCES [Thesis].[Experts]([Id])
);

-- NEW v4: Внешние рецензенты
CREATE TABLE [Thesis].[Reviewers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FullName] NVARCHAR(255) NOT NULL,
    [Position] NVARCHAR(255),
    [AcademicDegree] NVARCHAR(100),
    [Organization] NVARCHAR(255),
    [Email] NVARCHAR(255),
    [Phone] NVARCHAR(50),
    [IsActive] BIT DEFAULT 1
);

-- NEW v4: Рецензии
CREATE TABLE [Thesis].[Reviews] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [ReviewerId] INT NOT NULL,
    [ReviewText] NVARCHAR(MAX),
    [FileStoragePath] NVARCHAR(1000), -- Скан подписанной рецензии
    [UploadedBy] INT NOT NULL,
    [UploadedAt] DATETIME2,
    
    CONSTRAINT [FK_Review_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Review_Reviewer] FOREIGN KEY ([ReviewerId]) REFERENCES [Thesis].[Reviewers]([Id])
);

-- NEW v4: Отзывы научного руководителя
CREATE TABLE [Thesis].[SupervisorReviews] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [SupervisorId] INT NOT NULL,
    [ReviewText] NVARCHAR(MAX) NOT NULL,
    [FileStoragePath] NVARCHAR(1000),
    [CreatedAt] DATETIME2 DEFAULT SYSDATETIME(),
    
    CONSTRAINT [FK_SupReview_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_SupReview_Sup] FOREIGN KEY ([SupervisorId]) REFERENCES [Edu].[Staff]([Id])
);

-- =============================================
-- [Defense] ЗАЩИТА (EAV)
-- =============================================

-- MODIFIED v4: Комиссии
CREATE TABLE [Defense].[Commissions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [DepartmentId] INT NOT NULL,
    [AcademicYearId] INT NOT NULL,
    [Name] NVARCHAR(255),
    [CommissionType] NVARCHAR(50), -- 'PreDefense', 'GAK'
    [PreDefenseNumber] INT NULL, -- NEW v4: 1, 2, 3 для предзащит
    CONSTRAINT [FK_Comm_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id]),
    CONSTRAINT [FK_Comm_Year] FOREIGN KEY ([AcademicYearId]) REFERENCES [Common].[AcademicYears]([Id]),
    CONSTRAINT [Check_CommPreDef] CHECK ([PreDefenseNumber] IS NULL OR [PreDefenseNumber] BETWEEN 1 AND 3)
);

CREATE TABLE [Defense].[CommissionMembers] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [CommissionId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [RoleInCommission] NVARCHAR(100), -- 'Chairman', 'Secretary', 'Member'
    CONSTRAINT [FK_CommMem_Comm] FOREIGN KEY ([CommissionId]) REFERENCES [Defense].[Commissions]([Id]),
    CONSTRAINT [FK_CommMem_User] FOREIGN KEY ([UserId]) REFERENCES [Auth].[Users]([Id])
);

CREATE TABLE [Defense].[Schedules] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [CommissionId] INT NOT NULL,
    [WorkId] BIGINT NOT NULL,
    [DefenseDate] DATETIME2 NOT NULL,
    [Location] NVARCHAR(255),
    CONSTRAINT [FK_Sched_Comm] FOREIGN KEY ([CommissionId]) REFERENCES [Defense].[Commissions]([Id]),
    CONSTRAINT [FK_Sched_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id])
);

-- NEW v4: История попыток предзащит
CREATE TABLE [Defense].[PreDefenseAttempts] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId] BIGINT NOT NULL,
    [PreDefenseNumber] INT NOT NULL, -- 1, 2, 3
    [ScheduleId] BIGINT NULL, -- NULL если не явился
    [AttendanceStatus] NVARCHAR(50) DEFAULT 'Attended', -- 'Attended', 'Absent', 'Excused'
    [AverageScore] DECIMAL(5,2),
    [IsPassed] BIT DEFAULT 0,
    [AttemptDate] DATETIME2 DEFAULT SYSDATETIME(),
    
    CONSTRAINT [FK_PreDef_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_PreDef_Schedule] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id]),
    CONSTRAINT [Check_PreDefNum] CHECK ([PreDefenseNumber] BETWEEN 1 AND 3),
    CONSTRAINT [Check_Attendance] CHECK ([AttendanceStatus] IN ('Attended', 'Absent', 'Excused'))
);

-- Критерии (Настройка рубрик)
CREATE TABLE [Defense].[EvaluationCriteria] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [WorkTypeId] INT NOT NULL,
    [DepartmentId] INT NULL, -- NULL = стандарт вуза
    [CriteriaName] NVARCHAR(255) NOT NULL,
    [MaxScore] INT NOT NULL,
    [Weight] DECIMAL(3,2) DEFAULT 1.0,
    CONSTRAINT [FK_Crit_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [FK_Crit_Dept] FOREIGN KEY ([DepartmentId]) REFERENCES [Org].[Departments]([Id])
);

-- Оценки (Temporal для защиты от изменений)
CREATE TABLE [Defense].[Grades] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ScheduleId] BIGINT NOT NULL,
    [MemberId] INT NOT NULL,
    [CriteriaId] INT NOT NULL,
    [Score] INT NOT NULL,
    [Comment] NVARCHAR(MAX),

    [SysStartTime] DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN NOT NULL DEFAULT SYSDATETIME(),
    [SysEndTime] DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN NOT NULL DEFAULT CAST('9999-12-31 23:59:59.9999999' AS DATETIME2),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime]),

    CONSTRAINT [FK_Grades_Sched] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id]),
    CONSTRAINT [FK_Grades_Mem] FOREIGN KEY ([MemberId]) REFERENCES [Defense].[CommissionMembers]([Id]),
    CONSTRAINT [FK_Grades_Crit] FOREIGN KEY ([CriteriaId]) REFERENCES [Defense].[EvaluationCriteria]([Id]),
    CONSTRAINT [Check_Score_Positive] CHECK ([Score] >= 0)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [Defense].[GradesHistory]));

-- Итоговый протокол
CREATE TABLE [Defense].[Protocols] (
    [Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ScheduleId] BIGINT NOT NULL,
    [FinalScoreNumeric] DECIMAL(5,2),
    [FinalGradeLetter] NVARCHAR(5), -- 'A', 'A-', 'B+', etc.
    [Decision] NVARCHAR(MAX),
    [IsSigned] BIT DEFAULT 0,
    [ProtocolNumber] NVARCHAR(50),
    [ProtocolDate] DATETIME2,
    CONSTRAINT [FK_Proto_Sched] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id])
);
GO

-- =============================================
-- INDEXING (Performance Tuning)
-- =============================================

-- Индексы для Directions
CREATE INDEX [IX_Directions_Dept_Year] ON [Thesis].[Directions] 
    ([DepartmentId], [AcademicYearId], [CurrentStateId]) 
    INCLUDE ([SupervisorId]);

-- Индексы для Topics
CREATE INDEX [IX_Topics_Filter] ON [Thesis].[Topics] 
    ([DepartmentId], [AcademicYearId], [IsApproved])
    INCLUDE ([DirectionId], [SupervisorId]);

CREATE INDEX [IX_Topics_Direction] ON [Thesis].[Topics] ([DirectionId]);

-- Индексы для Applications
CREATE INDEX [IX_Applications_Status] ON [Thesis].[TopicApplications] 
    ([Status], [TopicId]) 
    INCLUDE ([StudentId], [AppliedAt]);

CREATE INDEX [IX_Applications_Student] ON [Thesis].[TopicApplications] ([StudentId], [Status]);

-- Индексы для StudentWorks (MODIFIED v4)
CREATE INDEX [IX_StudentWorks_Filter] ON [Thesis].[StudentWorks] 
    ([DepartmentId], [AcademicYearId], [CurrentStateId]) 
    INCLUDE ([TopicId]);

-- Индексы для WorkParticipants
CREATE INDEX [IX_Participants_Work] ON [Thesis].[WorkParticipants] ([WorkId]);
CREATE INDEX [IX_Participants_Student] ON [Thesis].[WorkParticipants] ([StudentId]);

-- Индексы для проверки прав
CREATE INDEX [IX_URA_UserCtx] ON [Auth].[UserRoleAssignments] 
    ([UserId], [DepartmentId]) 
    WHERE [ValidTo] IS NULL;

-- Индексы для файлов
CREATE INDEX [IX_Attach_Work] ON [Thesis].[Attachments] ([WorkId]);
CREATE INDEX [IX_Attach_Hash] ON [Thesis].[Attachments] ([FileHash]); -- NEW v4

-- Индексы для воркфлоу
CREATE INDEX [IX_Transitions_From] ON [Wf].[Transitions] ([FromStateId]);
CREATE INDEX [IX_WfHist_Work] ON [Thesis].[WorkflowHistory] ([WorkId], [TransitionDate] DESC); -- NEW v4

-- Индексы для QualityChecks
CREATE INDEX [IX_QualityChecks_Work] ON [Thesis].[QualityChecks] 
    ([WorkId], [CheckType], [AttemptNumber]);

-- Индексы для Experts
CREATE INDEX [IX_Experts_Type] ON [Thesis].[Experts] 
    ([DepartmentId], [ExpertiseType]) 
    WHERE [IsActive] = 1;

-- Индексы для Periods
CREATE INDEX [IX_Periods_Active] ON [Common].[Periods] 
    ([DepartmentId], [AcademicYearId], [WorkflowStage]) 
    WHERE [IsActive] = 1;

-- Индексы для PreDefenseAttempts
CREATE INDEX [IX_PreDefAttempts_Work] ON [Defense].[PreDefenseAttempts] 
    ([WorkId], [PreDefenseNumber]);

-- Индексы для Notifications
CREATE INDEX [IX_Notif_User_Unread] ON [Common].[Notifications] 
    ([UserId], [IsRead], [CreatedAt] DESC);

CREATE INDEX [IX_Notif_Entity] ON [Common].[Notifications] 
    ([RelatedEntityType], [RelatedEntityId]);

GO

PRINT 'ETMS Database Schema v4.0 created successfully.';
PRINT 'New features: Directions, Applications, Team Works, Enhanced Quality Checks, Periods, Notifications';
