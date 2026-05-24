using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Thesis");

            migrationBuilder.EnsureSchema(
                name: "Defense");

            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "Common");

            migrationBuilder.EnsureSchema(
                name: "Wf");

            migrationBuilder.CreateTable(
                name: "AttachmentTypes",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckTypes",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HasNumericResult = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    SpecialityId = table.Column<int>(type: "int", nullable: true),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CommissionTypeId = table.Column<int>(type: "int", nullable: false),
                    PreDefenseNumber = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.CheckConstraint("Check_Commission_PreDefNum", "[PreDefenseNumber] IS NULL OR [PreDefenseNumber] BETWEEN 1 AND 3");
                    table.ForeignKey(
                        name: "FK_Comm_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comm_Semester",
                        column: x => x.SemesterId,
                        principalTable: "Edu_Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comm_Speciality",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocalAccounts",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalAccounts_Edu_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TitleRu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TitleKz = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BodyTemplateRu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyTemplateKz = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyTemplateEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviewers",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AcademicDegree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleActionTypes",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperations",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOperations_RoleOperations_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Auth",
                        principalTable: "RoleOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessHistories",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStages",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkTypes",
                schema: "Wf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SpecialityLevelId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTypes_Level",
                        column: x => x.SpecialityLevelId,
                        principalTable: "Edu_SpecialityLevels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecialityCheckTypes",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialityId = table.Column<int>(type: "int", nullable: false),
                    CheckTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialityCheckTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecChecks_Speciality",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecChecks_Type",
                        column: x => x.CheckTypeId,
                        principalSchema: "Thesis",
                        principalTable: "CheckTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffAssignments",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false),
                    TargetEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetEntityId = table.Column<long>(type: "bigint", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    CommissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffAssignments_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalSchema: "Defense",
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RelatedEntityId = table.Column<long>(type: "bigint", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notif_Template",
                        column: x => x.TemplateId,
                        principalSchema: "Common",
                        principalTable: "NotificationTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Edu_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Edu_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_RoleAccesses_RoleAccessId",
                        column: x => x.RoleAccessId,
                        principalSchema: "Auth",
                        principalTable: "RoleAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperationActions",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    RoleOperationId = table.Column<int>(type: "int", nullable: false),
                    RoleActionTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleAccesses_RoleAccessId",
                        column: x => x.RoleAccessId,
                        principalSchema: "Auth",
                        principalTable: "RoleAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleActionTypes_RoleActionTypeId",
                        column: x => x.RoleActionTypeId,
                        principalSchema: "Auth",
                        principalTable: "RoleActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleOperations_RoleOperationId",
                        column: x => x.RoleOperationId,
                        principalSchema: "Auth",
                        principalTable: "RoleOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stages",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    SpecialityId = table.Column<int>(type: "int", nullable: true),
                    SemesterId = table.Column<int>(type: "int", nullable: false),
                    WorkflowStageId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stages", x => x.Id);
                    table.CheckConstraint("Check_Stage_Dates", "[EndDate] > [StartDate]");
                    table.ForeignKey(
                        name: "FK_Stages_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Semester",
                        column: x => x.SemesterId,
                        principalTable: "Edu_Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Speciality",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_WfStage",
                        column: x => x.WorkflowStageId,
                        principalSchema: "Common",
                        principalTable: "WorkflowStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationCriteria",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false),
                    OrgUnitId = table.Column<int>(type: "int", nullable: true),
                    SpecialityId = table.Column<int>(type: "int", nullable: true),
                    CriteriaName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 1.0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crit_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crit_Speciality",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crit_Type",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "States",
                schema: "Wf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false),
                    SystemName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_WorkType",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Directions",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleRu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleKz = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionRu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionKz = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CurrentStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    ReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysEndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysStartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directions_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_Semester",
                        column: x => x.SemesterId,
                        principalTable: "Edu_Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_State",
                        column: x => x.CurrentStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_Type",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateTable(
                name: "Transitions",
                schema: "Wf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromStateId = table.Column<int>(type: "int", nullable: false),
                    ToStateId = table.Column<int>(type: "int", nullable: false),
                    RoleAccessId = table.Column<int>(type: "int", nullable: true),
                    IsAutomatic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trans_From",
                        column: x => x.FromStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trans_Role",
                        column: x => x.RoleAccessId,
                        principalSchema: "Auth",
                        principalTable: "RoleAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trans_To",
                        column: x => x.ToStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DirectionId = table.Column<long>(type: "bigint", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    WorkTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SpecialityId = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleRu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TitleKz = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionRu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionKz = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsSubmittedForApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysEndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysStartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.CheckConstraint("Check_Participants_Positive", "[MaxParticipants] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Topics_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Direction",
                        column: x => x.DirectionId,
                        principalSchema: "Thesis",
                        principalTable: "Directions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Semester",
                        column: x => x.SemesterId,
                        principalTable: "Edu_Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Spec",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Type",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateTable(
                name: "StudentWorks",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    TopicId = table.Column<long>(type: "bigint", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SpecialityId = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CurrentStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    FinalGrade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsDefended = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysEndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysStartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Works_Dept",
                        column: x => x.OrgUnitId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Works_Semester",
                        column: x => x.SemesterId,
                        principalTable: "Edu_Semesters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Works_Speciality",
                        column: x => x.SpecialityId,
                        principalTable: "Edu_Specialities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Works_State",
                        column: x => x.CurrentStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Works_Topic",
                        column: x => x.TopicId,
                        principalSchema: "Thesis",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateTable(
                name: "TopicApplications",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SpecialityId = table.Column<int>(type: "int", nullable: true),
                    MotivationLetter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<int>(type: "int", nullable: true),
                    ReviewComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Reviewer",
                        column: x => x.ReviewedBy,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Student",
                        column: x => x.StudentId,
                        principalTable: "Edu_Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_Topic",
                        column: x => x.TopicId,
                        principalSchema: "Thesis",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    AttachmentTypeId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileStoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    AttachmentTypeId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attach_State",
                        column: x => x.StateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attach_Type",
                        column: x => x.AttachmentTypeId,
                        principalSchema: "Thesis",
                        principalTable: "AttachmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attach_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_AttachmentTypes_AttachmentTypeId1",
                        column: x => x.AttachmentTypeId1,
                        principalSchema: "Thesis",
                        principalTable: "AttachmentTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    CheckTypeId = table.Column<int>(type: "int", nullable: false),
                    AssignedExpertId = table.Column<int>(type: "int", nullable: true),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResultValue = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Check_Type",
                        column: x => x.CheckTypeId,
                        principalSchema: "Thesis",
                        principalTable: "CheckTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Check_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    DefenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsReconciliationStarted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sched_Comm",
                        column: x => x.CommissionId,
                        principalSchema: "Defense",
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sched_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowHistory",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    FromStateId = table.Column<int>(type: "int", nullable: true),
                    ToStateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WfHist_FromState",
                        column: x => x.FromStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WfHist_ToState",
                        column: x => x.ToStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WfHist_User",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WfHist_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkParticipants",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Student",
                        column: x => x.StudentId,
                        principalTable: "Edu_Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkReviews",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorUserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkReviews_StudentWorks_WorkId",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    AssignmentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CriteriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    Score = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    CreatedBy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysEndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SysStartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.CheckConstraint("Check_Score_Positive", "[Score] >= 0");
                    table.ForeignKey(
                        name: "FK_Grades_Assignment",
                        column: x => x.AssignmentId,
                        principalSchema: "Common",
                        principalTable: "StaffAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Crit",
                        column: x => x.CriteriaId,
                        principalSchema: "Defense",
                        principalTable: "EvaluationCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Sched",
                        column: x => x.ScheduleId,
                        principalSchema: "Defense",
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.CreateTable(
                name: "PreDefenseAttempts",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    PreDefenseNumber = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: true),
                    AttendanceStatusId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    AverageScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AttemptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreDefenseAttempts", x => x.Id);
                    table.CheckConstraint("Check_PreDefNum", "[PreDefenseNumber] BETWEEN 1 AND 3");
                    table.ForeignKey(
                        name: "FK_PreDef_Schedule",
                        column: x => x.ScheduleId,
                        principalSchema: "Defense",
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreDef_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Protocols",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    ProtocolDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FinalizedBy = table.Column<int>(type: "int", nullable: true),
                    FinalizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalScoreNumeric = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    FinalGradeLetter = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProtocolNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Protocols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Protocols_Commission",
                        column: x => x.CommissionId,
                        principalSchema: "Defense",
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Protocols_Finalizer",
                        column: x => x.FinalizedBy,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Protocols_Schedule",
                        column: x => x.ScheduleId,
                        principalSchema: "Defense",
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attach_Hash",
                schema: "Thesis",
                table: "Attachments",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_Attach_Work",
                schema: "Thesis",
                table: "Attachments",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentTypeId",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_StateId",
                schema: "Thesis",
                table: "Attachments",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_OrgUnitId",
                schema: "Defense",
                table: "Commissions",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_SemesterId",
                schema: "Defense",
                table: "Commissions",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_SpecialityId",
                schema: "Defense",
                table: "Commissions",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_CurrentStateId",
                schema: "Thesis",
                table: "Directions",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_Dept_Year",
                schema: "Thesis",
                table: "Directions",
                columns: new[] { "OrgUnitId", "SemesterId", "CurrentStateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Directions_SemesterId",
                schema: "Thesis",
                table: "Directions",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_WorkTypeId",
                schema: "Thesis",
                table: "Directions",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_OrgUnitId",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_SpecialityId",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_WorkTypeId",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_AssignmentId",
                schema: "Defense",
                table: "Grades",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CriteriaId",
                schema: "Defense",
                table: "Grades",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "UQ_Grade_Schedule_Assignment_Criteria",
                schema: "Defense",
                table: "Grades",
                columns: new[] { "ScheduleId", "AssignmentId", "CriteriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_LocalAccount_UserId",
                schema: "Auth",
                table: "LocalAccounts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notif_Entity",
                schema: "Common",
                table: "Notifications",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notif_User_Unread",
                schema: "Common",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TemplateId",
                schema: "Common",
                table: "Notifications",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "UQ_Template_Event",
                schema: "Common",
                table: "NotificationTemplates",
                column: "EventType",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PreDefAttempts_Work",
                schema: "Defense",
                table: "PreDefenseAttempts",
                columns: new[] { "WorkId", "PreDefenseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreDefenseAttempts_ScheduleId",
                schema: "Defense",
                table: "PreDefenseAttempts",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_CommissionId",
                schema: "Defense",
                table: "Protocols",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Protocols_FinalizedBy",
                schema: "Defense",
                table: "Protocols",
                column: "FinalizedBy");

            migrationBuilder.CreateIndex(
                name: "UQ_Protocol_Schedule",
                schema: "Defense",
                table: "Protocols",
                column: "ScheduleId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_CheckTypeId",
                schema: "Thesis",
                table: "QualityChecks",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_Work",
                schema: "Thesis",
                table: "QualityChecks",
                columns: new[] { "WorkId", "CheckTypeId", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "UQ_RoleAccess_Code",
                schema: "Auth",
                table: "RoleAccesses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_RoleActionType_Code",
                schema: "Auth",
                table: "RoleActionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationActions_RoleActionTypeId",
                schema: "Auth",
                table: "RoleOperationActions",
                column: "RoleActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationActions_RoleOperationId",
                schema: "Auth",
                table: "RoleOperationActions",
                column: "RoleOperationId");

            migrationBuilder.CreateIndex(
                name: "UQ_RoleOperationAction",
                schema: "Auth",
                table: "RoleOperationActions",
                columns: new[] { "RoleAccessId", "RoleOperationId", "RoleActionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperations_Tree",
                schema: "Auth",
                table: "RoleOperations",
                columns: new[] { "ParentId", "OrderBy" });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_WorkId",
                schema: "Defense",
                table: "Schedules",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "UQ_Schedule_Commission_Work",
                schema: "Defense",
                table: "Schedules",
                columns: new[] { "CommissionId", "WorkId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialityCheckTypes_CheckTypeId",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "UQ_Speciality_CheckType",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                columns: new[] { "SpecialityId", "CheckTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_CommissionId",
                schema: "Common",
                table: "StaffAssignments",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_IsActive",
                schema: "Common",
                table: "StaffAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_TargetEntityType_TargetEntityId",
                schema: "Common",
                table: "StaffAssignments",
                columns: new[] { "TargetEntityType", "TargetEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_UserId",
                schema: "Common",
                table: "StaffAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_Active",
                schema: "Common",
                table: "Stages",
                columns: new[] { "OrgUnitId", "SpecialityId", "SemesterId", "WorkflowStageId" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_SemesterId",
                schema: "Common",
                table: "Stages",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_SpecialityId",
                schema: "Common",
                table: "Stages",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_WorkflowStageId",
                schema: "Common",
                table: "Stages",
                column: "WorkflowStageId");

            migrationBuilder.CreateIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States",
                columns: new[] { "WorkTypeId", "SystemName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_CurrentStateId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_Filter",
                schema: "Thesis",
                table: "StudentWorks",
                columns: new[] { "OrgUnitId", "SemesterId", "CurrentStateId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_SemesterId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_SpecialityId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_TopicId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications",
                columns: new[] { "StatusId", "TopicId" });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications",
                columns: new[] { "StudentId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_TopicApplications_ReviewedBy",
                schema: "Thesis",
                table: "TopicApplications",
                column: "ReviewedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TopicApplications_TopicId",
                schema: "Thesis",
                table: "TopicApplications",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Direction",
                schema: "Thesis",
                table: "Topics",
                column: "DirectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics",
                columns: new[] { "OrgUnitId", "SemesterId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SemesterId",
                schema: "Thesis",
                table: "Topics",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SpecialityId",
                schema: "Thesis",
                table: "Topics",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_WorkTypeId",
                schema: "Thesis",
                table: "Topics",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_From",
                schema: "Wf",
                table: "Transitions",
                column: "FromStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_RoleAccessId",
                schema: "Wf",
                table: "Transitions",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_ToStateId",
                schema: "Wf",
                table: "Transitions",
                column: "ToStateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_RoleAccessId",
                schema: "Auth",
                table: "UserAccesses",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserAccess",
                schema: "Auth",
                table: "UserAccesses",
                columns: new[] { "UserId", "RoleAccessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessHistory_Role",
                schema: "Auth",
                table: "UserAccessHistories",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessHistory_User",
                schema: "Auth",
                table: "UserAccessHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WfHist_Work",
                schema: "Thesis",
                table: "WorkflowHistory",
                columns: new[] { "WorkId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowHistory_FromStateId",
                schema: "Thesis",
                table: "WorkflowHistory",
                column: "FromStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowHistory_ToStateId",
                schema: "Thesis",
                table: "WorkflowHistory",
                column: "ToStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowHistory_UserId",
                schema: "Thesis",
                table: "WorkflowHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Student",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Work",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "UQ_Participant_Work_Student",
                schema: "Thesis",
                table: "WorkParticipants",
                columns: new[] { "WorkId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkReviews_WorkId",
                schema: "Thesis",
                table: "WorkReviews",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTypes_SpecialityLevelId",
                schema: "Wf",
                table: "WorkTypes",
                column: "SpecialityLevelId");

            migrationBuilder.CreateIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachments",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Grades",
                schema: "Defense")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropTable(
                name: "LocalAccounts",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "PreDefenseAttempts",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "Protocols",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "QualityChecks",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Reviewers",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "RoleOperationActions",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "SpecialityCheckTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Stages",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "TopicApplications",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Transitions",
                schema: "Wf");

            migrationBuilder.DropTable(
                name: "UserAccesses",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "UserAccessHistories",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "WorkflowHistory",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "WorkParticipants",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "WorkReviews",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "AttachmentTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "StaffAssignments",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "EvaluationCriteria",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "NotificationTemplates",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "Schedules",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "RoleActionTypes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleOperations",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "CheckTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "WorkflowStages",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "RoleAccesses",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Commissions",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "StudentWorks",
                schema: "Thesis")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropTable(
                name: "Topics",
                schema: "Thesis")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropTable(
                name: "Directions",
                schema: "Thesis")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropTable(
                name: "States",
                schema: "Wf");

            migrationBuilder.DropTable(
                name: "WorkTypes",
                schema: "Wf");
        }
    }
}
