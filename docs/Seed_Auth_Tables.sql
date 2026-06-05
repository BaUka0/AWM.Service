USE awm_database;
GO

DECLARE @BcryptHash NVARCHAR(255) = '$2a$11$oQtFZXEajoHb5MYgkYDxkutpewhXKpwvAVZ0Tnq6txMQ11AW8d25S';
INSERT INTO [Auth].[LocalAccounts] ([UserId], [PasswordHash], [IsActive], [CreatedAt], [CreatedBy])
SELECT ID, @BcryptHash, 1, SYSDATETIME(), 0 FROM [Edu_Users];
GO
DECLARE @AdminRoleId INT = (SELECT Id FROM [Auth].[RoleAccesses] WHERE Code = 'ADMIN');
DECLARE @DeptHeadRoleId INT = (SELECT Id FROM [Auth].[RoleAccesses] WHERE Code = 'DEPARTMENT_HEAD');
DECLARE @StudentRoleId INT = (SELECT Id FROM [Auth].[RoleAccesses] WHERE Code = 'STUDENT');
INSERT INTO [Auth].[UserAccesses] ([UserId], [RoleAccessId], [AssignedBy], [AssignedAt], [CreatedAt], [CreatedBy]) VALUES (1, @AdminRoleId, 0, SYSDATETIME(), SYSDATETIME(), 0);
INSERT INTO [Auth].[UserAccesses] ([UserId], [RoleAccessId], [AssignedBy], [AssignedAt], [CreatedAt], [CreatedBy]) VALUES (2, @DeptHeadRoleId, 0, SYSDATETIME(), SYSDATETIME(), 0);
INSERT INTO [Auth].[UserAccesses] ([UserId], [RoleAccessId], [AssignedBy], [AssignedAt], [CreatedAt], [CreatedBy])
SELECT StudentID, @StudentRoleId, 0, SYSDATETIME(), SYSDATETIME(), 0 FROM [Edu_Students];
GO
PRINT 'New University Data with Auth Seeded Successfully.';
GO