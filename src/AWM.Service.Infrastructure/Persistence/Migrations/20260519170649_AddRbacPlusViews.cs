using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[UserAccessMatrix];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[RoleAccessMatrix];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[ReducedUserAccessMatrix];");

            migrationBuilder.Sql(@"
CREATE VIEW [Auth].[UserAccessMatrix] AS
SELECT 
    ua.[UserId],
    ra.[Code] AS RoleCode,
    ro.[Name] AS OperationName,
    rat.[Code] AS ActionTypeName
FROM [Auth].[UserAccesses] ua
JOIN [Auth].[RoleAccesses] ra ON ua.[RoleAccessId] = ra.[Id]
JOIN [Auth].[RoleOperationActions] roa ON ra.[Id] = roa.[RoleAccessId]
JOIN [Auth].[RoleOperations] ro ON roa.[RoleOperationId] = ro.[Id]
JOIN [Auth].[RoleActionTypes] rat ON roa.[RoleActionTypeId] = rat.[Id];
");

            migrationBuilder.Sql(@"
CREATE VIEW [Auth].[RoleAccessMatrix] AS
SELECT 
    ra.[Code] AS RoleCode,
    ro.[Name] AS OperationName,
    rat.[Code] AS ActionTypeName
FROM [Auth].[RoleAccesses] ra
JOIN [Auth].[RoleOperationActions] roa ON ra.[Id] = roa.[RoleAccessId]
JOIN [Auth].[RoleOperations] ro ON roa.[RoleOperationId] = ro.[Id]
JOIN [Auth].[RoleActionTypes] rat ON roa.[RoleActionTypeId] = rat.[Id];
");

            migrationBuilder.Sql(@"
CREATE VIEW [Auth].[ReducedUserAccessMatrix] AS
SELECT DISTINCT
    ua.[UserId],
    ra.[Code] AS RoleCode
FROM [Auth].[UserAccesses] ua
JOIN [Auth].[RoleAccesses] ra ON ua.[RoleAccessId] = ra.[Id];
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[UserAccessMatrix];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[RoleAccessMatrix];");
            migrationBuilder.Sql("DROP VIEW IF EXISTS [Auth].[ReducedUserAccessMatrix];");
        }
    }
}
