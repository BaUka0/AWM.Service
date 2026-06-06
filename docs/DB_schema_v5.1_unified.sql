/*
============================================================================
PROJECT: AWM v5.1 Unified Schema
DESCRIPTION: Academic Work Management System — addon tables for university DB
DBMS: Microsoft SQL Server 2016+
UPDATED: 2026-05-31
NOTES:
  - Edu_* foundation tables live in DB_scheam_university.sql (run first)
  - All user FKs point to Edu_Users.ID (university master)
  - All student FKs point to Edu_Students.StudentID
  - Department = Edu_OrgUnits WHERE TypeID = 1 (Kafedra)
  - Institute = Edu_OrgUnits WHERE TypeID = 2
  - Semesters = Edu_Semesters (foundation)
  - Column names match C# property names unless EF HasColumnName() overrides
  - Local accounts linked via Auth.LocalAccounts.UserId → Edu_Users.ID
  - IAuditable: CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
  - ISoftDeletable: IsDeleted, DeletedAt, DeletedBy
============================================================================
*/

-- =============================================
-- SCHEMAS
-- =============================================
GO
CREATE SCHEMA [Auth];
GO
CREATE SCHEMA [Common];
GO
CREATE SCHEMA [Wf];
GO
CREATE SCHEMA [Thesis];
GO
CREATE SCHEMA [Defense];
GO

-- =============================================
-- [Auth] HYBRID AUTH + RBAC+
-- =============================================

CREATE TABLE [Auth].[LocalAccounts] (
    [Id]                    INT IDENTITY(1,1) PRIMARY KEY,
    [UserId]                INT NOT NULL,
    [PasswordHash]          NVARCHAR(500) NOT NULL,
    [RefreshToken]          NVARCHAR(500),
    [RefreshTokenExpiryTime] DATETIME2,
    [IsActive]              BIT NOT NULL DEFAULT 1,
    -- IAuditable
    [CreatedAt]             DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]             INT NOT NULL,
    [LastModifiedAt]        DATETIME2,
    [LastModifiedBy]        INT,
    CONSTRAINT [FK_LocalAccounts_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [UQ_LocalAccount_UserId] UNIQUE ([UserId])
);

CREATE TABLE [Auth].[RoleAccess] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [Code]            NVARCHAR(50) NOT NULL,
    [NameRu]          NVARCHAR(100) NOT NULL,
    [NameKz]          NVARCHAR(100) NOT NULL,
    [NameEn]          NVARCHAR(100) NOT NULL,
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [UQ_RoleAccess_Code] UNIQUE ([Code])
);

CREATE TABLE [Auth].[RoleOperation] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [ParentId]        INT NULL,
    [Name]            NVARCHAR(100) NOT NULL,
    [NameRu]          NVARCHAR(255) NOT NULL,
    [NameKz]          NVARCHAR(255) NOT NULL,
    [NameEn]          NVARCHAR(255) NOT NULL,
    [OrderBy]         INT NOT NULL DEFAULT 0,
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_RoleOperation_Parent] FOREIGN KEY ([ParentId]) REFERENCES [Auth].[RoleOperation]([Id])
);

CREATE TABLE [Auth].[RoleActionType] (
    [Id]     INT IDENTITY(1,1) PRIMARY KEY,
    [Code]   NVARCHAR(20) NOT NULL,
    [NameRu] NVARCHAR(50) NOT NULL,
    [NameKz] NVARCHAR(50) NOT NULL,
    [NameEn] NVARCHAR(50) NOT NULL,
    CONSTRAINT [UQ_RoleActionType_Code] UNIQUE ([Code])
);

CREATE TABLE [Auth].[RoleOperationAction] (
    [Id]               INT IDENTITY(1,1) PRIMARY KEY,
    [RoleAccessId]     INT NOT NULL,
    [RoleOperationId]  INT NOT NULL,
    [RoleActionTypeId] INT NOT NULL,
    CONSTRAINT [FK_ROA_Role] FOREIGN KEY ([RoleAccessId]) REFERENCES [Auth].[RoleAccess]([Id]),
    CONSTRAINT [FK_ROA_Operation] FOREIGN KEY ([RoleOperationId]) REFERENCES [Auth].[RoleOperation]([Id]),
    CONSTRAINT [FK_ROA_Action] FOREIGN KEY ([RoleActionTypeId]) REFERENCES [Auth].[RoleActionType]([Id]),
    CONSTRAINT [UQ_ROA] UNIQUE ([RoleAccessId], [RoleOperationId], [RoleActionTypeId])
);

CREATE TABLE [Auth].[UserAccess] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [UserId]          INT NOT NULL,
    [RoleAccessId]    INT NOT NULL,
    [AssignedBy]      INT NULL,
    [AssignedAt]      DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_UA_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [FK_UA_Role] FOREIGN KEY ([RoleAccessId]) REFERENCES [Auth].[RoleAccess]([Id]),
    CONSTRAINT [FK_UA_Assigner] FOREIGN KEY ([AssignedBy]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [UQ_UserAccess] UNIQUE ([UserId], [RoleAccessId])
);

CREATE TABLE [Auth].[UserAccessHistory] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [UserId]          INT NOT NULL,
    [RoleAccessId]    INT NOT NULL,
    [Action]          NVARCHAR(20) NOT NULL,
    [AssignedBy]      INT NULL,
    [AssignedAt]      DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_UAH_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [FK_UAH_Role] FOREIGN KEY ([RoleAccessId]) REFERENCES [Auth].[RoleAccess]([Id]),
    CONSTRAINT [FK_UAH_Assigner] FOREIGN KEY ([AssignedBy]) REFERENCES [Edu_Users]([ID])
);

GO

-- RBAC+ Views
CREATE VIEW [Auth].[UserAccessMatrix] AS
SELECT 
    ua.[UserId],
    ra.[Code] AS RoleCode,
    ro.[Name] AS OperationName,
    rat.[Code] AS ActionTypeCode
FROM [Auth].[UserAccess] ua
JOIN [Auth].[RoleAccess] ra ON ua.[RoleAccessId] = ra.[Id]
JOIN [Auth].[RoleOperationAction] roa ON ra.[Id] = roa.[RoleAccessId]
JOIN [Auth].[RoleOperation] ro ON roa.[RoleOperationId] = ro.[Id]
JOIN [Auth].[RoleActionType] rat ON roa.[RoleActionTypeId] = rat.[Id];
GO

CREATE VIEW [Auth].[RoleAccessMatrix] AS
SELECT 
    ra.[Code] AS RoleCode,
    ro.[Name] AS OperationName,
    rat.[Code] AS ActionTypeCode
FROM [Auth].[RoleAccess] ra
JOIN [Auth].[RoleOperationAction] roa ON ra.[Id] = roa.[RoleAccessId]
JOIN [Auth].[RoleOperation] ro ON roa.[RoleOperationId] = ro.[Id]
JOIN [Auth].[RoleActionType] rat ON roa.[RoleActionTypeId] = rat.[Id];
GO

CREATE VIEW [Auth].[ReducedUserAccessMatrix] AS
SELECT DISTINCT
    ua.[UserId],
    ra.[Code] AS RoleCode
FROM [Auth].[UserAccess] ua
JOIN [Auth].[RoleAccess] ra ON ua.[RoleAccessId] = ra.[Id];
GO

-- =============================================
-- [Wf] WORKFLOW ENGINE
-- =============================================

CREATE TABLE [Wf].[WorkTypes] (
    [Id]                 INT IDENTITY(1,1) PRIMARY KEY,
    [Name]               NVARCHAR(100) NOT NULL,
    [SpecialityLevelId]  INT NULL,
    -- IAuditable & ISoftDeletable
    [CreatedAt]          DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]          INT NOT NULL DEFAULT 1,
    [LastModifiedAt]     DATETIME2,
    [LastModifiedBy]     INT,
    [IsDeleted]          BIT NOT NULL DEFAULT 0,
    [DeletedAt]          DATETIME2,
    [DeletedBy]          INT,
    CONSTRAINT [FK_WorkTypes_Level] FOREIGN KEY ([SpecialityLevelId]) REFERENCES [Edu_SpecialityLevels]([ID])
);

CREATE TABLE [Wf].[States] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [WorkTypeId]      INT NOT NULL,
    [SystemName]      NVARCHAR(100) NOT NULL,
    [DisplayName]     NVARCHAR(100),
    [IsFinal]         BIT NOT NULL DEFAULT 0,
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 1,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [FK_States_WorkType] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id])
);

CREATE TABLE [Wf].[Transitions] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [FromStateId]     INT NOT NULL,
    [ToStateId]       INT NOT NULL,
    [RoleAccessId]    INT NULL,
    [IsAutomatic]     BIT NOT NULL DEFAULT 0,
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 1,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [FK_Trans_From] FOREIGN KEY ([FromStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Trans_To] FOREIGN KEY ([ToStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Trans_Role] FOREIGN KEY ([RoleAccessId]) REFERENCES [Auth].[RoleAccess]([Id])
);

-- =============================================
-- [Common] REFERENCE & SHARED TABLES
-- =============================================

CREATE TABLE [Common].[WorkflowStages] (
    [Id]      INT IDENTITY(1,1) PRIMARY KEY,
    [Name]    NVARCHAR(100) NOT NULL,
    [OrderBy] INT NOT NULL
);

CREATE TABLE [Common].[Stages] (
    [Id]               INT IDENTITY(1,1) PRIMARY KEY,
    [OrgUnitId]        INT NOT NULL,
    [SpecialityId]     INT NULL,
    [SemesterId]       INT NOT NULL,
    [WorkflowStageId]  INT NOT NULL,
    [StartDate]        DATETIME2 NOT NULL,
    [EndDate]          DATETIME2 NOT NULL,
    [IsActive]         BIT NOT NULL DEFAULT 1,
    -- IAuditable & ISoftDeletable
    [CreatedAt]        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]        INT NOT NULL,
    [LastModifiedAt]   DATETIME2,
    [LastModifiedBy]   INT,
    [IsDeleted]        BIT NOT NULL DEFAULT 0,
    [DeletedAt]        DATETIME2,
    [DeletedBy]        INT,
    CONSTRAINT [FK_Stages_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Stages_Speciality] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID]),
    CONSTRAINT [FK_Stages_Semester] FOREIGN KEY ([SemesterId]) REFERENCES [Edu_Semesters]([ID]),
    CONSTRAINT [FK_Stages_WfStage] FOREIGN KEY ([WorkflowStageId]) REFERENCES [Common].[WorkflowStages]([Id]),
    CONSTRAINT [FK_Stages_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [Check_Stage_Dates] CHECK ([EndDate] > [StartDate])
);

CREATE TABLE [Common].[StaffAssignments] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId]            INT NOT NULL,
    [RoleType]          INT NOT NULL,
    [TargetEntityType]  NVARCHAR(50) NOT NULL,
    [TargetEntityId]    BIGINT NOT NULL,
    [MetadataJson]      NVARCHAR(MAX),
    [ValidFrom]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [ValidTo]           DATETIME2,
    [IsActive]          BIT NOT NULL DEFAULT 1,
    -- IAuditable & ISoftDeletable
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL,
    [LastModifiedAt]    DATETIME2,
    [LastModifiedBy]    INT,
    [IsDeleted]         BIT NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2,
    [DeletedBy]         INT,
    CONSTRAINT [FK_StaffAssign_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID])
);

CREATE TABLE [Common].[NotificationTemplates] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [EventType]       NVARCHAR(100) NOT NULL,
    [TitleRu]         NVARCHAR(255),
    [TitleKz]         NVARCHAR(255),
    [TitleEn]         NVARCHAR(255),
    [BodyTemplateRu]  NVARCHAR(MAX),
    [BodyTemplateKz]  NVARCHAR(MAX),
    [BodyTemplateEn]  NVARCHAR(MAX),
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 1,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [UQ_Template_Event] UNIQUE ([EventType])
);

CREATE TABLE [Common].[Notifications] (
    [Id]                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    [UserId]              INT NOT NULL,
    [TemplateId]          INT NULL,
    [Title]               NVARCHAR(255) NOT NULL,
    [Body]                NVARCHAR(MAX),
    [RelatedEntityType]   NVARCHAR(50),
    [RelatedEntityId]     BIGINT,
    [IsRead]              BIT NOT NULL DEFAULT 0,
    -- IAuditable
    [CreatedAt]           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]           INT NOT NULL DEFAULT 1,
    [LastModifiedAt]      DATETIME2,
    [LastModifiedBy]      INT,
    CONSTRAINT [FK_Notif_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [FK_Notif_Template] FOREIGN KEY ([TemplateId]) REFERENCES [Common].[NotificationTemplates]([Id])
);

-- =============================================
-- [Thesis] REFERENCE TABLES
-- =============================================

CREATE TABLE [Thesis].[ApplicationStatuses] (
    [Id]   INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Thesis].[ParticipantRoles] (
    [Id]   INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Thesis].[AttachmentTypes] (
    [Id]    INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(100) NOT NULL,
    [Code]  NVARCHAR(50)
);

CREATE TABLE [Thesis].[CheckTypes] (
    [Id]               INT IDENTITY(1,1) PRIMARY KEY,
    [Title]            NVARCHAR(100) NOT NULL,
    [HasNumericResult]  BIT NOT NULL DEFAULT 0,
    [Code]             NVARCHAR(50)
);

CREATE TABLE [Thesis].[SpecialityCheckTypes] (
    [Id]                INT IDENTITY(1,1) PRIMARY KEY,
    [OrgUnitId]         INT NOT NULL,
    [SpecialityId]      INT NULL,
    [CheckTypeId]       INT NOT NULL,
    [MinimumPassValue]  DECIMAL(5,2),
    [IsActive]          BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_SpecChecks_OrgUnit] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_SpecChecks_Speciality] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID]),
    CONSTRAINT [FK_SpecChecks_Type] FOREIGN KEY ([CheckTypeId]) REFERENCES [Thesis].[CheckTypes]([Id]),
    CONSTRAINT [UQ_OrgUnit_Speciality_CheckType] UNIQUE ([OrgUnitId], [SpecialityId], [CheckTypeId])
);

-- =============================================
-- [Thesis] DIRECTIONS & TOPICS
-- =============================================

CREATE TABLE [Thesis].[Directions] (
    [Id]              BIGINT IDENTITY(1,1) PRIMARY KEY,
    [OrgUnitId]       INT NOT NULL,
    [SemesterId]      INT NOT NULL,
    [WorkTypeId]      INT NOT NULL,
    [TitleRu]         NVARCHAR(500) NOT NULL,
    [TitleEn]         NVARCHAR(500),
    [TitleKz]         NVARCHAR(500),
    [DescriptionRu]   NVARCHAR(MAX),
    [DescriptionKz]   NVARCHAR(MAX),
    [DescriptionEn]   NVARCHAR(MAX),
    [CurrentStateId]  INT NOT NULL,
    [SubmittedAt]     DATETIME2,
    [ReviewedAt]      DATETIME2,
    [ReviewedBy]      INT NULL,
    [ReviewComment]   NVARCHAR(MAX),
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [LastModifiedBy]  INT NOT NULL,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [FK_Directions_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Directions_Semester] FOREIGN KEY ([SemesterId]) REFERENCES [Edu_Semesters]([ID]),
    CONSTRAINT [FK_Directions_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [FK_Directions_State] FOREIGN KEY ([CurrentStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Directions_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [FK_Directions_Reviewer] FOREIGN KEY ([ReviewedBy]) REFERENCES [Edu_Users]([ID])
);

-- NOTE: Topics is a temporal table in EF Core (SysStartTime/SysEndTime, history: TopicsHistory)
CREATE TABLE [Thesis].[Topics] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [DirectionId]       BIGINT NULL,
    [SemesterId]        INT NOT NULL,
    [OrgUnitId]         INT NOT NULL,
    [WorkTypeId]        INT NOT NULL,
    [SpecialityId]      INT NULL,
    [TitleRu]           NVARCHAR(500) NOT NULL,
    [TitleEn]           NVARCHAR(500),
    [TitleKz]           NVARCHAR(500),
    [DescriptionRu]     NVARCHAR(MAX),
    [DescriptionKz]     NVARCHAR(MAX),
    [DescriptionEn]     NVARCHAR(MAX),
    [MaxParticipants]   INT NOT NULL DEFAULT 1,
    [Status]            INT NOT NULL DEFAULT 0,  -- TopicStatus enum: 0=Draft, 1=Pending, ...
    [ReviewComment]     NVARCHAR(MAX),
    [ReviewedBy]        INT NULL,
    [ReviewedAt]        DATETIME2,
    -- IAuditable & ISoftDeletable
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL,
    [LastModifiedAt]    DATETIME2,
    [LastModifiedBy]    INT,
    [IsDeleted]         BIT NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2,
    [DeletedBy]         INT,
    CONSTRAINT [FK_Topics_Direction] FOREIGN KEY ([DirectionId]) REFERENCES [Thesis].[Directions]([Id]),
    CONSTRAINT [FK_Topics_Semester] FOREIGN KEY ([SemesterId]) REFERENCES [Edu_Semesters]([ID]),
    CONSTRAINT [FK_Topics_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Topics_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [FK_Topics_Spec] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID]),
    CONSTRAINT [FK_Topics_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [Check_Participants_Positive] CHECK ([MaxParticipants] BETWEEN 1 AND 3)
);

CREATE TABLE [Thesis].[TopicApplications] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TopicId]           BIGINT NOT NULL,
    [StudentId]         INT NOT NULL,
    [SpecialityId]      INT NULL,
    [MotivationLetter]  NVARCHAR(MAX),
    [AppliedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [StatusId]          INT NOT NULL DEFAULT 1,
    [ReviewedAt]        DATETIME2,
    [ReviewedBy]        INT NULL,
    [ReviewComment]     NVARCHAR(MAX),
    -- IAuditable & ISoftDeletable
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL,
    [LastModifiedAt]    DATETIME2,
    [LastModifiedBy]    INT,
    [IsDeleted]         BIT NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2,
    [DeletedBy]         INT,
    CONSTRAINT [FK_Applications_Topic] FOREIGN KEY ([TopicId]) REFERENCES [Thesis].[Topics]([Id]),
    CONSTRAINT [FK_Applications_Student] FOREIGN KEY ([StudentId]) REFERENCES [Edu_Students]([StudentID]),
    CONSTRAINT [FK_Applications_Status] FOREIGN KEY ([StatusId]) REFERENCES [Thesis].[ApplicationStatuses]([Id]),
    CONSTRAINT [FK_Applications_Reviewer] FOREIGN KEY ([ReviewedBy]) REFERENCES [Edu_Users]([ID])
);

-- =============================================
-- [Thesis] STUDENT WORKS & PARTICIPANTS
-- =============================================

CREATE TABLE [Thesis].[StudentWorks] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [TopicId]           BIGINT NULL,
    [SemesterId]        INT NOT NULL,
    [OrgUnitId]         INT NOT NULL,
    [SpecialityId]      INT NULL,
    [CurrentStateId]    INT NOT NULL,
    [FinalGrade]        NVARCHAR(10),
    [IsDefended]        BIT NOT NULL DEFAULT 0,
    [MetadataJson]      NVARCHAR(MAX),
    -- IAuditable & ISoftDeletable
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL,
    [LastModifiedAt]    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [LastModifiedBy]    INT NOT NULL,
    [IsDeleted]         BIT NOT NULL DEFAULT 0,
    [DeletedAt]         DATETIME2,
    [DeletedBy]         INT,
    CONSTRAINT [UQ_Works_Topic] UNIQUE ([TopicId]),
    CONSTRAINT [FK_Works_Topic] FOREIGN KEY ([TopicId]) REFERENCES [Thesis].[Topics]([Id]),
    CONSTRAINT [FK_Works_Semester] FOREIGN KEY ([SemesterId]) REFERENCES [Edu_Semesters]([ID]),
    CONSTRAINT [FK_Works_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Works_Speciality] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID]),
    CONSTRAINT [FK_Works_State] FOREIGN KEY ([CurrentStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Works_Creator] FOREIGN KEY ([CreatedBy]) REFERENCES [Edu_Users]([ID]),
    CONSTRAINT [FK_Works_Updater] FOREIGN KEY ([LastModifiedBy]) REFERENCES [Edu_Users]([ID])
);

CREATE TABLE [Thesis].[WorkParticipants] (
    [Id]              BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]          BIGINT NOT NULL,
    [StudentId]       INT NOT NULL,
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 0,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_Participants_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Participants_Student] FOREIGN KEY ([StudentId]) REFERENCES [Edu_Students]([StudentID]),
    CONSTRAINT [UQ_Participant_Work_Student] UNIQUE ([WorkId], [StudentId])
);

CREATE TABLE [Thesis].[WorkflowHistory] (
    [Id]              BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]          BIGINT NOT NULL,
    [FromStateId]     INT NULL,
    [ToStateId]       INT NOT NULL,
    [UserId]          INT NOT NULL,
    [Comment]         NVARCHAR(MAX),
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_WfHist_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_WfHist_From] FOREIGN KEY ([FromStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_WfHist_To] FOREIGN KEY ([ToStateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_WfHist_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID])
);

-- =============================================
-- [Thesis] ATTACHMENTS & QUALITY
-- =============================================

CREATE TABLE [Thesis].[Attachments] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]            BIGINT NOT NULL,
    [StateId]           INT NULL,
    [AttachmentTypeId]  INT NOT NULL,
    [FileName]          NVARCHAR(255) NOT NULL,
    [FileStoragePath]   NVARCHAR(500) NOT NULL,
    [FileHash]          CHAR(64) NOT NULL,
    [FileSizeBytes]     BIGINT NOT NULL,
    [ContentType]       NVARCHAR(100) NOT NULL,
    -- IAuditable (CreatedBy = uploader)
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL,
    [LastModifiedAt]    DATETIME2,
    [LastModifiedBy]    INT,
    CONSTRAINT [FK_Attach_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Attach_State] FOREIGN KEY ([StateId]) REFERENCES [Wf].[States]([Id]),
    CONSTRAINT [FK_Attach_Type] FOREIGN KEY ([AttachmentTypeId]) REFERENCES [Thesis].[AttachmentTypes]([Id]),
    CONSTRAINT [FK_Attach_Uploader] FOREIGN KEY ([CreatedBy]) REFERENCES [Edu_Users]([ID])
);

CREATE TABLE [Thesis].[QualityChecks] (
    [Id]                BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]            BIGINT NOT NULL,
    [CheckTypeId]       INT NOT NULL,
    [AssignedExpertId]  INT NULL,
    [AttachmentId]      BIGINT NULL,
    [AttemptNumber]     INT NOT NULL DEFAULT 1,
    [IsPassed]          BIT NOT NULL DEFAULT 0,
    [ResultValue]       DECIMAL(5,2),
    [Comment]           NVARCHAR(MAX),
    -- IAuditable
    [CreatedAt]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]         INT NOT NULL DEFAULT 1,
    [LastModifiedAt]    DATETIME2,
    [LastModifiedBy]    INT,
    CONSTRAINT [FK_Check_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_Check_Type] FOREIGN KEY ([CheckTypeId]) REFERENCES [Thesis].[CheckTypes]([Id]),
    CONSTRAINT [FK_Check_Attachment] FOREIGN KEY ([AttachmentId]) REFERENCES [Thesis].[Attachments]([Id])
);

-- =============================================
-- [Thesis] REVIEWS & REVIEWERS
-- =============================================

CREATE TABLE [Thesis].[Reviewers] (
    [Id]              INT IDENTITY(1,1) PRIMARY KEY,
    [FullName]        NVARCHAR(255) NOT NULL,
    [Position]        NVARCHAR(255),
    [AcademicDegree]  NVARCHAR(100),
    [Organization]    NVARCHAR(255),
    [Email]           NVARCHAR(255),
    [Phone]           NVARCHAR(50),
    [IsActive]        BIT NOT NULL DEFAULT 1,
    [UserId]          INT NULL,
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [FK_Reviewers_User] FOREIGN KEY ([UserId]) REFERENCES [Edu_Users]([ID])
);

CREATE TABLE [Thesis].[WorkReviews] (
    [Id]              BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]          BIGINT NOT NULL,
    [AuthorUserId]    INT NOT NULL,
    [Type]            INT NOT NULL,  -- ReviewType enum: 1=Supervisor, 2=External, 3=Expert
    [ReviewText]      NVARCHAR(MAX) NOT NULL,
    [MetadataJson]    NVARCHAR(MAX),
    [IsFinal]         BIT NOT NULL DEFAULT 0,
    -- IAuditable & ISoftDeletable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 1,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    [IsDeleted]       BIT NOT NULL DEFAULT 0,
    [DeletedAt]       DATETIME2,
    [DeletedBy]       INT,
    CONSTRAINT [FK_WorkReview_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_WorkReview_Author] FOREIGN KEY ([AuthorUserId]) REFERENCES [Edu_Users]([ID])
);

-- =============================================
-- [Defense] COMMISSIONS & SCHEDULES
-- =============================================

CREATE TABLE [Defense].[CommissionTypes] (
    [Id]   INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Defense].[CommissionRoles] (
    [Id]   INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Defense].[AttendanceStatuses] (
    [Id]   INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL
);

CREATE TABLE [Defense].[Commissions] (
    [Id]                 INT IDENTITY(1,1) PRIMARY KEY,
    [OrgUnitId]          INT NOT NULL,
    [SpecialityId]       INT NULL,
    [SemesterId]         INT NOT NULL,
    [Name]               NVARCHAR(255),
    [CommissionTypeId]   INT NOT NULL,
    [PreDefenseNumber]   INT NULL,
    -- IAuditable & ISoftDeletable
    [CreatedAt]          DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]          INT NOT NULL DEFAULT 1,
    [LastModifiedAt]     DATETIME2,
    [LastModifiedBy]     INT,
    [IsDeleted]          BIT NOT NULL DEFAULT 0,
    [DeletedAt]          DATETIME2,
    [DeletedBy]          INT,
    CONSTRAINT [FK_Comm_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Comm_Speciality] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID]),
    CONSTRAINT [FK_Comm_Semester] FOREIGN KEY ([SemesterId]) REFERENCES [Edu_Semesters]([ID]),
    CONSTRAINT [FK_Comm_Type] FOREIGN KEY ([CommissionTypeId]) REFERENCES [Defense].[CommissionTypes]([Id]),
    CONSTRAINT [Check_CommPreDef] CHECK ([PreDefenseNumber] IS NULL OR [PreDefenseNumber] BETWEEN 1 AND 3)
);

CREATE TABLE [Defense].[Schedules] (
    [Id]                      BIGINT IDENTITY(1,1) PRIMARY KEY,
    [CommissionId]            INT NOT NULL,
    [WorkId]                  BIGINT NOT NULL,
    [DefenseDate]             DATETIME2 NOT NULL,
    [Location]                NVARCHAR(255),
    [IsReconciliationStarted] BIT NOT NULL DEFAULT 0,
    -- IAuditable & ISoftDeletable
    [CreatedAt]               DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]               INT NOT NULL DEFAULT 1,
    [LastModifiedAt]          DATETIME2,
    [LastModifiedBy]          INT,
    [IsDeleted]               BIT NOT NULL DEFAULT 0,
    [DeletedAt]               DATETIME2,
    [DeletedBy]               INT,
    CONSTRAINT [FK_Sched_Comm] FOREIGN KEY ([CommissionId]) REFERENCES [Defense].[Commissions]([Id]),
    CONSTRAINT [FK_Sched_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id])
);

CREATE TABLE [Defense].[PreDefenseAttempts] (
    [Id]                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    [WorkId]              BIGINT NOT NULL,
    [PreDefenseNumber]    INT NOT NULL,
    [ScheduleId]          BIGINT NULL,
    [AttendanceStatusId]  INT NOT NULL DEFAULT 1,
    [AverageScore]        DECIMAL(5,2),
    [IsPassed]            BIT NOT NULL DEFAULT 0,
    [AttemptDate]         DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    -- IAuditable
    [CreatedAt]           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]           INT NOT NULL DEFAULT 1,
    [LastModifiedAt]      DATETIME2,
    [LastModifiedBy]      INT,
    CONSTRAINT [FK_PreDef_Work] FOREIGN KEY ([WorkId]) REFERENCES [Thesis].[StudentWorks]([Id]),
    CONSTRAINT [FK_PreDef_Schedule] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id]),
    CONSTRAINT [FK_PreDef_Status] FOREIGN KEY ([AttendanceStatusId]) REFERENCES [Defense].[AttendanceStatuses]([Id]),
    CONSTRAINT [Check_PreDefNum] CHECK ([PreDefenseNumber] BETWEEN 1 AND 3)
);

-- =============================================
-- [Defense] EVALUATION & GRADING
-- =============================================

CREATE TABLE [Defense].[EvaluationCriteria] (
    [Id]                  INT IDENTITY(1,1) PRIMARY KEY,
    [WorkTypeId]          INT NOT NULL,
    [OrgUnitId]           INT NULL,
    [SpecialityId]        INT NULL,
    [CriteriaName]        NVARCHAR(255) NOT NULL,
    [MaxScore]            INT NOT NULL,
    [Weight]              DECIMAL(5,2) NOT NULL DEFAULT 1.0,
    [DefenseStageType]    INT NULL,
    [SortOrder]           INT NOT NULL DEFAULT 0,
    -- IAuditable & ISoftDeletable
    [CreatedAt]           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]           INT NOT NULL DEFAULT 1,
    [LastModifiedAt]      DATETIME2,
    [LastModifiedBy]      INT,
    [IsDeleted]           BIT NOT NULL DEFAULT 0,
    [DeletedAt]           DATETIME2,
    [DeletedBy]           INT,
    CONSTRAINT [FK_Crit_Type] FOREIGN KEY ([WorkTypeId]) REFERENCES [Wf].[WorkTypes]([Id]),
    CONSTRAINT [FK_Crit_Dept] FOREIGN KEY ([OrgUnitId]) REFERENCES [Edu_OrgUnits]([ID]),
    CONSTRAINT [FK_Crit_Speciality] FOREIGN KEY ([SpecialityId]) REFERENCES [Edu_Specialities]([ID])
);

CREATE TABLE [Defense].[Grades] (
    [Id]              BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ScheduleId]      BIGINT NOT NULL,
    [AssignmentId]    BIGINT NOT NULL,
    [CriteriaId]      INT NOT NULL,
    [Score]           INT NOT NULL,
    [Comment]         NVARCHAR(MAX),
    -- IAuditable
    [CreatedAt]       DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]       INT NOT NULL DEFAULT 1,
    [LastModifiedAt]  DATETIME2,
    [LastModifiedBy]  INT,
    CONSTRAINT [FK_Grades_Sched] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id]),
    CONSTRAINT [FK_Grades_Assignment] FOREIGN KEY ([AssignmentId]) REFERENCES [Common].[StaffAssignments]([Id]),
    CONSTRAINT [FK_Grades_Crit] FOREIGN KEY ([CriteriaId]) REFERENCES [Defense].[EvaluationCriteria]([Id]),
    CONSTRAINT [Check_Score_Positive] CHECK ([Score] >= 0)
);

CREATE TABLE [Defense].[Protocols] (
    [Id]                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    [ScheduleId]          BIGINT NOT NULL,
    [CommissionId]        INT NOT NULL,
    [FinalScoreNumeric]   DECIMAL(5,2),
    [FinalGradeLetter]    NVARCHAR(5),
    [Decision]            NVARCHAR(MAX),
    [IsSigned]            BIT NOT NULL DEFAULT 0,   -- mapped from domain IsFinalized
    [ProtocolNumber]      NVARCHAR(50),
    [ProtocolDate]        DATETIME2 NOT NULL,        -- mapped from domain SessionDate
    [DocumentPath]        NVARCHAR(500),
    [FinalizedBy]         INT NULL,
    [FinalizedAt]         DATETIME2,
    [DecisionType]        INT NULL,
    [ReadinessPercent]    INT NULL,
    [Comments]            NVARCHAR(MAX),
    -- IAuditable & ISoftDeletable
    [CreatedAt]           DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    [CreatedBy]           INT NOT NULL DEFAULT 1,
    [LastModifiedAt]      DATETIME2,
    [LastModifiedBy]      INT,
    [IsDeleted]           BIT NOT NULL DEFAULT 0,
    [DeletedAt]           DATETIME2,
    [DeletedBy]           INT,
    CONSTRAINT [FK_Protocols_Schedule] FOREIGN KEY ([ScheduleId]) REFERENCES [Defense].[Schedules]([Id]),
    CONSTRAINT [FK_Protocols_Commission] FOREIGN KEY ([CommissionId]) REFERENCES [Defense].[Commissions]([Id]),
    CONSTRAINT [FK_Protocols_Finalizer] FOREIGN KEY ([FinalizedBy]) REFERENCES [Edu_Users]([ID])
);

GO

-- =============================================
-- INDEXING (Performance Tuning)
-- =============================================

CREATE INDEX [IX_Directions_Dept_Year] ON [Thesis].[Directions] 
    ([OrgUnitId], [SemesterId], [CurrentStateId]);

CREATE INDEX [IX_Topics_Filter] ON [Thesis].[Topics] 
    ([OrgUnitId], [SemesterId], [Status])
    INCLUDE ([DirectionId]);

CREATE INDEX [IX_Topics_Direction] ON [Thesis].[Topics] ([DirectionId]);

CREATE INDEX [IX_Applications_Status] ON [Thesis].[TopicApplications] 
    ([StatusId], [TopicId]) 
    INCLUDE ([StudentId], [AppliedAt]);

CREATE INDEX [IX_Applications_Student] ON [Thesis].[TopicApplications] ([StudentId], [StatusId]);

CREATE INDEX [IX_StudentWorks_Filter] ON [Thesis].[StudentWorks] 
    ([OrgUnitId], [SemesterId], [CurrentStateId]) 
    INCLUDE ([TopicId]);

CREATE INDEX [IX_Participants_Work] ON [Thesis].[WorkParticipants] ([WorkId]);
CREATE INDEX [IX_Participants_Student] ON [Thesis].[WorkParticipants] ([StudentId]);

CREATE INDEX [IX_UA_User] ON [Auth].[UserAccess] ([UserId], [RoleAccessId]);

CREATE INDEX [IX_Attach_Work] ON [Thesis].[Attachments] ([WorkId]);
CREATE INDEX [IX_Attach_Hash] ON [Thesis].[Attachments] ([FileHash]);

CREATE INDEX [IX_Transitions_From] ON [Wf].[Transitions] ([FromStateId]);
CREATE INDEX [IX_WfHist_Work] ON [Thesis].[WorkflowHistory] ([WorkId], [CreatedAt] DESC);

CREATE INDEX [IX_QualityChecks_Work] ON [Thesis].[QualityChecks] 
    ([WorkId], [CheckTypeId], [AttemptNumber]);

CREATE INDEX [IX_Stages_Active] ON [Common].[Stages] 
    ([OrgUnitId], [SpecialityId], [SemesterId], [WorkflowStageId]) 
    WHERE [IsActive] = 1;

CREATE INDEX [IX_PreDefAttempts_Work] ON [Defense].[PreDefenseAttempts] 
    ([WorkId], [PreDefenseNumber]);

CREATE INDEX [IX_Notif_User_Unread] ON [Common].[Notifications] 
    ([UserId], [IsRead], [CreatedAt] DESC);

CREATE INDEX [IX_Notif_Entity] ON [Common].[Notifications] 
    ([RelatedEntityType], [RelatedEntityId]);

GO

PRINT 'AWM v5.1 Unified Schema created successfully.';
PRINT 'Addon: Auth, Common, Wf, Thesis, Defense schemas.';
PRINT 'Edu_* foundation tables are in DB_scheam_university.sql.';
