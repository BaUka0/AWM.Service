using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Edu");

            migrationBuilder.EnsureSchema(
                name: "Common");

            migrationBuilder.EnsureSchema(
                name: "Thesis");

            migrationBuilder.EnsureSchema(
                name: "Defense");

            migrationBuilder.EnsureSchema(
                name: "Org");

            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "Wf");

            migrationBuilder.CreateTable(
                name: "DegreeLevels",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationYears = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegreeLevels", x => x.Id);
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
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AcademicDegree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Organization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Reviewers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ScopeLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkTypes",
                schema: "Wf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DegreeLevelId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_WorkType_Degree",
                        column: x => x.DegreeLevelId,
                        principalSchema: "Edu",
                        principalTable: "DegreeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicYears_University",
                        column: x => x.UniversityId,
                        principalSchema: "Org",
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Institutes",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Institutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Institutes_University",
                        column: x => x.UniversityId,
                        principalSchema: "Org",
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniversityId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_University",
                        column: x => x.UniversityId,
                        principalSchema: "Org",
                        principalTable: "Universities",
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
                name: "Departments",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstituteId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Institute",
                        column: x => x.InstituteId,
                        principalSchema: "Org",
                        principalTable: "Institutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transitions",
                schema: "Wf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromStateId = table.Column<int>(type: "int", nullable: false),
                    ToStateId = table.Column<int>(type: "int", nullable: false),
                    AllowedRoleId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_Trans_FromState",
                        column: x => x.FromStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trans_Role",
                        column: x => x.AllowedRoleId,
                        principalSchema: "Auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trans_ToState",
                        column: x => x.ToStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcademicPrograms",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DegreeLevelId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_AcademicPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_Degree",
                        column: x => x.DegreeLevelId,
                        principalSchema: "Edu",
                        principalTable: "DegreeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Programs_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CommissionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                        name: "FK_Commissions_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
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
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_Criteria_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Criteria_WorkType",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Experts",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    ExpertiseType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Experts", x => x.Id);
                    table.CheckConstraint("Check_Expert_Type", "[ExpertiseType] IN ('NormControl', 'SoftwareCheck', 'AntiPlagiarism')");
                    table.ForeignKey(
                        name: "FK_Experts_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experts_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    WorkflowStage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_Periods", x => x.Id);
                    table.CheckConstraint("Check_Period_Dates", "[EndDate] > [StartDate]");
                    table.ForeignKey(
                        name: "FK_Periods_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Periods_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AcademicDegree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaxStudentsLoad = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
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
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Staff_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleAssignments",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    InstituteId = table.Column<int>(type: "int", nullable: true),
                    AcademicYearId = table.Column<int>(type: "int", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_URA_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_Institute",
                        column: x => x.InstituteId,
                        principalSchema: "Org",
                        principalTable: "Institutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_Role",
                        column: x => x.RoleId,
                        principalSchema: "Auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_URA_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    AdmissionYear = table.Column<int>(type: "int", nullable: false),
                    CurrentCourse = table.Column<int>(type: "int", nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Program",
                        column: x => x.ProgramId,
                        principalSchema: "Edu",
                        principalTable: "AcademicPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissionMembers",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleInCommission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommMembers_Commission",
                        column: x => x.CommissionId,
                        principalSchema: "Defense",
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommMembers_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
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
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SupervisorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false)
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_State",
                        column: x => x.CurrentStateId,
                        principalSchema: "Wf",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_Supervisor",
                        column: x => x.SupervisorId,
                        principalSchema: "Edu",
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_WorkType",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Directions_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

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
                    AcademicYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    SupervisorId = table.Column<int>(type: "int", nullable: false)
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Direction",
                        column: x => x.DirectionId,
                        principalSchema: "Thesis",
                        principalTable: "Directions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Supervisor",
                        column: x => x.SupervisorId,
                        principalSchema: "Edu",
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_WorkType",
                        column: x => x.WorkTypeId,
                        principalSchema: "Wf",
                        principalTable: "WorkTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topics_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
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
                    AcademicYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
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
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
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
                    table.ForeignKey(
                        name: "FK_Works_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
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
                    MotivationLetter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Submitted"),
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
                    table.CheckConstraint("Check_Application_Status", "[Status] IN ('Submitted', 'Accepted', 'Rejected', 'Withdrawn')");
                    table.ForeignKey(
                        name: "FK_Applications_Student",
                        column: x => x.StudentId,
                        principalSchema: "Edu",
                        principalTable: "Students",
                        principalColumn: "Id",
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
                    AttachmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileStoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Attach_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    CheckType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                        name: "FK_QChecks_Expert",
                        column: x => x.AssignedExpertId,
                        principalSchema: "Thesis",
                        principalTable: "Experts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QChecks_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileStoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Reviewer",
                        column: x => x.ReviewerId,
                        principalSchema: "Thesis",
                        principalTable: "Reviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Work",
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
                        name: "FK_Schedules_Commission",
                        column: x => x.CommissionId,
                        principalSchema: "Defense",
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Work",
                        column: x => x.WorkId,
                        principalSchema: "Thesis",
                        principalTable: "StudentWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupervisorReviews",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileStoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_SupervisorReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupReviews_Supervisor",
                        column: x => x.SupervisorId,
                        principalSchema: "Edu",
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupReviews_Work",
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
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
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
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                        principalSchema: "Edu",
                        principalTable: "Students",
                        principalColumn: "Id",
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
                    MemberId = table.Column<int>(type: "int", nullable: false)
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
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_Grades_Criteria",
                        column: x => x.CriteriaId,
                        principalSchema: "Defense",
                        principalTable: "EvaluationCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Member",
                        column: x => x.MemberId,
                        principalSchema: "Defense",
                        principalTable: "CommissionMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Schedule",
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
                    AttendanceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Attended"),
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
                    table.CheckConstraint("Check_PreDef_Attendance", "[AttendanceStatus] IN ('Attended', 'Absent', 'Excused')");
                    table.CheckConstraint("Check_PreDef_Num", "[PreDefenseNumber] BETWEEN 1 AND 3");
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
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FinalizedBy = table.Column<int>(type: "int", nullable: true),
                    FinalizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
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
                name: "IX_AcademicPrograms_DegreeLevelId",
                schema: "Edu",
                table: "AcademicPrograms",
                column: "DegreeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPrograms_DepartmentId",
                schema: "Edu",
                table: "AcademicPrograms",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_UniversityId",
                schema: "Common",
                table: "AcademicYears",
                column: "UniversityId");

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
                name: "IX_Attachments_StateId",
                schema: "Thesis",
                table: "Attachments",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionMembers_UserId",
                schema: "Defense",
                table: "CommissionMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_CommMember_Commission_User",
                schema: "Defense",
                table: "CommissionMembers",
                columns: new[] { "CommissionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_AcademicYearId",
                schema: "Defense",
                table: "Commissions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_DepartmentId",
                schema: "Defense",
                table: "Commissions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_InstituteId",
                schema: "Org",
                table: "Departments",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_AcademicYearId",
                schema: "Thesis",
                table: "Directions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_CurrentStateId",
                schema: "Thesis",
                table: "Directions",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_Dept_Year",
                schema: "Thesis",
                table: "Directions",
                columns: new[] { "DepartmentId", "AcademicYearId", "CurrentStateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Directions_SupervisorId",
                schema: "Thesis",
                table: "Directions",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_WorkTypeId",
                schema: "Thesis",
                table: "Directions",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_DepartmentId",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationCriteria_WorkTypeId",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_Type",
                schema: "Thesis",
                table: "Experts",
                columns: new[] { "DepartmentId", "ExpertiseType" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_UserId",
                schema: "Thesis",
                table: "Experts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CriteriaId",
                schema: "Defense",
                table: "Grades",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_MemberId",
                schema: "Defense",
                table: "Grades",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "UQ_Grade_Schedule_Member_Criteria",
                schema: "Defense",
                table: "Grades",
                columns: new[] { "ScheduleId", "MemberId", "CriteriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Institutes_UniversityId",
                schema: "Org",
                table: "Institutes",
                column: "UniversityId");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Periods_AcademicYearId",
                schema: "Common",
                table: "Periods",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Periods_Active",
                schema: "Common",
                table: "Periods",
                columns: new[] { "DepartmentId", "AcademicYearId", "WorkflowStage" },
                filter: "[IsActive] = 1");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_AssignedExpertId",
                schema: "Thesis",
                table: "QualityChecks",
                column: "AssignedExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_Work",
                schema: "Thesis",
                table: "QualityChecks",
                columns: new[] { "WorkId", "CheckType", "AttemptNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                schema: "Thesis",
                table: "Reviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_WorkId",
                schema: "Thesis",
                table: "Reviews",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "UQ_Role_SystemName",
                schema: "Auth",
                table: "Roles",
                column: "SystemName",
                unique: true);

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_DepartmentId",
                schema: "Edu",
                table: "Staff",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UQ_Staff_User",
                schema: "Edu",
                table: "Staff",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States",
                columns: new[] { "WorkTypeId", "SystemName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProgramId",
                schema: "Edu",
                table: "Students",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "UQ_Student_User",
                schema: "Edu",
                table: "Students",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_AcademicYearId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_CurrentStateId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_Filter",
                schema: "Thesis",
                table: "StudentWorks",
                columns: new[] { "DepartmentId", "AcademicYearId", "CurrentStateId" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_TopicId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorReviews_SupervisorId",
                schema: "Thesis",
                table: "SupervisorReviews",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "UQ_SupReview_Work_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews",
                columns: new[] { "WorkId", "SupervisorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications",
                columns: new[] { "Status", "TopicId" });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TopicApplications_TopicId",
                schema: "Thesis",
                table: "TopicApplications",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AcademicYearId",
                schema: "Thesis",
                table: "Topics",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Direction",
                schema: "Thesis",
                table: "Topics",
                column: "DirectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics",
                columns: new[] { "DepartmentId", "AcademicYearId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SupervisorId",
                schema: "Thesis",
                table: "Topics",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_WorkTypeId",
                schema: "Thesis",
                table: "Topics",
                column: "WorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_AllowedRoleId",
                schema: "Wf",
                table: "Transitions",
                column: "AllowedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_From",
                schema: "Wf",
                table: "Transitions",
                column: "FromStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Transitions_ToStateId",
                schema: "Wf",
                table: "Transitions",
                column: "ToStateId");

            migrationBuilder.CreateIndex(
                name: "UQ_University_Code",
                schema: "Org",
                table: "Universities",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_URA_UserCtx",
                schema: "Auth",
                table: "UserRoleAssignments",
                columns: new[] { "UserId", "DepartmentId" },
                filter: "[DepartmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_AcademicYearId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_DepartmentId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_InstituteId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_RoleId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                schema: "Auth",
                table: "Users",
                columns: new[] { "UniversityId", "Email" },
                unique: true);

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
                name: "IX_WorkTypes_DegreeLevelId",
                schema: "Wf",
                table: "WorkTypes",
                column: "DegreeLevelId");

            migrationBuilder.CreateIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes",
                column: "Name",
                unique: true);
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
                name: "Notifications",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "Periods",
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
                name: "Reviews",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "SupervisorReviews",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "TopicApplications",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Transitions",
                schema: "Wf");

            migrationBuilder.DropTable(
                name: "UserRoleAssignments",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "WorkflowHistory",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "WorkParticipants",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "EvaluationCriteria",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "CommissionMembers",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "NotificationTemplates",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "Schedules",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "Experts",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Reviewers",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "Edu");

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
                name: "AcademicPrograms",
                schema: "Edu");

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
                name: "Staff",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "AcademicYears",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "WorkTypes",
                schema: "Wf");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "Org");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "DegreeLevels",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "Institutes",
                schema: "Org");

            migrationBuilder.DropTable(
                name: "Universities",
                schema: "Org");
        }
    }
}
