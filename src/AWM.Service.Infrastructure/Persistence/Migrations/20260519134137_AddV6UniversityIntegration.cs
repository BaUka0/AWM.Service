using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddV6UniversityIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommMembers_User",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Dept",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Dept",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Supervisor",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Criteria_Dept",
                schema: "Defense",
                table: "EvaluationCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Experts_Dept",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropForeignKey(
                name: "FK_Experts_User",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                schema: "Common",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Finalizer",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_Dept",
                schema: "Common",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_Semester",
                schema: "Common",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Dept",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_SupReviews_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Dept",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Supervisor",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Trans_Role",
                schema: "Wf",
                table: "Transitions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Users_UserId",
                schema: "Auth",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_WfHist_User",
                schema: "Thesis",
                table: "WorkflowHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Student",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkType_Degree",
                schema: "Wf",
                table: "WorkTypes");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Semesters",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "Staff",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "SemesterTypes",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "AcademicPrograms",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "StudentStatuses",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "DegreeLevels",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "Org");

            migrationBuilder.DropTable(
                name: "Institutes",
                schema: "Org");

            migrationBuilder.AddForeignKey(
                name: "FK_CommMembers_User",
                schema: "Defense",
                table: "CommissionMembers",
                column: "UserId",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Dept",
                schema: "Defense",
                table: "Commissions",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Dept",
                schema: "Thesis",
                table: "Directions",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Supervisor",
                schema: "Thesis",
                table: "Directions",
                column: "SupervisorId",
                principalTable: "Edu_Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Criteria_Dept",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experts_Dept",
                schema: "Thesis",
                table: "Experts",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experts_User",
                schema: "Thesis",
                table: "Experts",
                column: "UserId",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Edu_Users_UserId",
                schema: "Common",
                table: "Notifications",
                column: "UserId",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Finalizer",
                schema: "Defense",
                table: "Protocols",
                column: "FinalizedBy",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_Dept",
                schema: "Common",
                table: "Stages",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_Semester",
                schema: "Common",
                table: "Stages",
                column: "SemesterId",
                principalTable: "Edu_Semesters",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Dept",
                schema: "Thesis",
                table: "StudentWorks",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupReviews_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews",
                column: "SupervisorId",
                principalTable: "Edu_Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications",
                column: "StudentId",
                principalTable: "Edu_Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Dept",
                schema: "Thesis",
                table: "Topics",
                column: "DepartmentId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Supervisor",
                schema: "Thesis",
                table: "Topics",
                column: "SupervisorId",
                principalTable: "Edu_Employees",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trans_Role",
                schema: "Wf",
                table: "Transitions",
                column: "AllowedRoleId",
                principalSchema: "Auth",
                principalTable: "RoleAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Edu_Users_UserId",
                schema: "Auth",
                table: "UserAccesses",
                column: "UserId",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WfHist_User",
                schema: "Thesis",
                table: "WorkflowHistory",
                column: "UserId",
                principalTable: "Edu_Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Student",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "StudentId",
                principalTable: "Edu_Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkType_Degree",
                schema: "Wf",
                table: "WorkTypes",
                column: "DegreeLevelId",
                principalTable: "Edu_SpecialityLevels",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommMembers_User",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Dept",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Dept",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Supervisor",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Criteria_Dept",
                schema: "Defense",
                table: "EvaluationCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Experts_Dept",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropForeignKey(
                name: "FK_Experts_User",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Edu_Users_UserId",
                schema: "Common",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Protocols_Finalizer",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_Dept",
                schema: "Common",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Stages_Semester",
                schema: "Common",
                table: "Stages");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Dept",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_SupReviews_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Dept",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Supervisor",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Trans_Role",
                schema: "Wf",
                table: "Transitions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Edu_Users_UserId",
                schema: "Auth",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_WfHist_User",
                schema: "Thesis",
                table: "WorkflowHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Student",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkType_Degree",
                schema: "Wf",
                table: "WorkTypes");

            migrationBuilder.EnsureSchema(
                name: "Edu");

            migrationBuilder.EnsureSchema(
                name: "Org");

            migrationBuilder.CreateTable(
                name: "DegreeLevels",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DurationYears = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegreeLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Institutes",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    SystemName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemesterTypes",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderBy = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentStatuses",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "Org",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    InstituteId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
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
                name: "Semesters",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    SemesterTypeId = table.Column<int>(type: "int", nullable: false),
                    StartsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudyYear = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Semesters_SemesterType",
                        column: x => x.SemesterTypeId,
                        principalSchema: "Edu",
                        principalTable: "SemesterTypes",
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
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DegreeLevelId = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
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
                name: "Staff",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicDegree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsSupervisor = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    MaxStudentsLoad = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    Position = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                name: "Students",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdmissionYear = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CurrentCourse = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    GroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ProgramId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_Students_Status",
                        column: x => x.StatusId,
                        principalSchema: "Edu",
                        principalTable: "StudentStatuses",
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
                name: "IX_Departments_InstituteId",
                schema: "Org",
                table: "Departments",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "UQ_Role_SystemName",
                schema: "Auth",
                table: "Roles",
                column: "SystemName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_SemesterTypeId",
                schema: "Edu",
                table: "Semesters",
                column: "SemesterTypeId");

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
                name: "IX_Students_ProgramId",
                schema: "Edu",
                table: "Students",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StatusId",
                schema: "Edu",
                table: "Students",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_Student_User",
                schema: "Edu",
                table: "Students",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                schema: "Auth",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CommMembers_User",
                schema: "Defense",
                table: "CommissionMembers",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Dept",
                schema: "Defense",
                table: "Commissions",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Dept",
                schema: "Thesis",
                table: "Directions",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Supervisor",
                schema: "Thesis",
                table: "Directions",
                column: "SupervisorId",
                principalSchema: "Edu",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Criteria_Dept",
                schema: "Defense",
                table: "EvaluationCriteria",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experts_Dept",
                schema: "Thesis",
                table: "Experts",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experts_User",
                schema: "Thesis",
                table: "Experts",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                schema: "Common",
                table: "Notifications",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Protocols_Finalizer",
                schema: "Defense",
                table: "Protocols",
                column: "FinalizedBy",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_Dept",
                schema: "Common",
                table: "Stages",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stages_Semester",
                schema: "Common",
                table: "Stages",
                column: "SemesterId",
                principalSchema: "Edu",
                principalTable: "Semesters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Dept",
                schema: "Thesis",
                table: "StudentWorks",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupReviews_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews",
                column: "SupervisorId",
                principalSchema: "Edu",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications",
                column: "StudentId",
                principalSchema: "Edu",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Dept",
                schema: "Thesis",
                table: "Topics",
                column: "DepartmentId",
                principalSchema: "Org",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Supervisor",
                schema: "Thesis",
                table: "Topics",
                column: "SupervisorId",
                principalSchema: "Edu",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trans_Role",
                schema: "Wf",
                table: "Transitions",
                column: "AllowedRoleId",
                principalSchema: "Auth",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Users_UserId",
                schema: "Auth",
                table: "UserAccesses",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WfHist_User",
                schema: "Thesis",
                table: "WorkflowHistory",
                column: "UserId",
                principalSchema: "Auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Student",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "StudentId",
                principalSchema: "Edu",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkType_Degree",
                schema: "Wf",
                table: "WorkTypes",
                column: "DegreeLevelId",
                principalSchema: "Edu",
                principalTable: "DegreeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
