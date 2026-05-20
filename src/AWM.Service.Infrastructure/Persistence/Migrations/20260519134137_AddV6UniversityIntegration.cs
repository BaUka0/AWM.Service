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
            // Create legacy university tables (must exist before FKs reference them)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Users (
        ID            int           NOT NULL PRIMARY KEY,
        LastName      nvarchar(max) NOT NULL,
        FirstName     nvarchar(max),
        MiddleName    nvarchar(max),
        Email         nvarchar(max),
        DOB           date,
        Male          bit,
        MobilePhone   nvarchar(max),
        IIN           nvarchar(max),
        PhotoFileName nvarchar(255)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_OrgUnitTypes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_OrgUnitTypes (
        ID    int           NOT NULL PRIMARY KEY,
        Title nvarchar(max)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_OrgUnits' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_OrgUnits (
        ID         int           NOT NULL PRIMARY KEY,
        ParentID   int,
        Title      nvarchar(max) NOT NULL,
        Deleted    bit           NOT NULL,
        ShortTitle nvarchar(max),
        TypeID     int           NOT NULL,
        CONSTRAINT FK_Edu_OrgUnits_ParentID_Edu_OrgUnits FOREIGN KEY (ParentID) REFERENCES Edu_OrgUnits(ID),
        CONSTRAINT FK_Edu_OrgUnits_TypeID_Edu_OrgUnitTypes FOREIGN KEY (TypeID) REFERENCES Edu_OrgUnitTypes(ID)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Employees' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Employees (
        ID        int NOT NULL PRIMARY KEY,
        IsAdvisor bit NOT NULL,
        CONSTRAINT FK_Edu_Employees_ID_Edu_Users FOREIGN KEY (ID) REFERENCES Edu_Users(ID)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_SemesterTypes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_SemesterTypes (
        ID      int           NOT NULL PRIMARY KEY,
        Title   nvarchar(max) NOT NULL,
        OrderBy int           NOT NULL
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Semesters' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Semesters (
        ID             int           NOT NULL PRIMARY KEY,
        Title          nvarchar(max) NOT NULL,
        StartsOn       datetime2     NOT NULL,
        EndsOn         datetime2     NOT NULL,
        StudyYear      int           NOT NULL,
        SemesterTypeID int           NOT NULL,
        CONSTRAINT FK_Edu_Semesters_SemesterTypeID_Edu_SemesterTypes FOREIGN KEY (SemesterTypeID) REFERENCES Edu_SemesterTypes(ID)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_SpecialityLevels' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_SpecialityLevels (
        ID     int           NOT NULL PRIMARY KEY,
        Title  nvarchar(max) NOT NULL,
        NoBDID nvarchar(max)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Specialities' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Specialities (
        ID           int           NOT NULL PRIMARY KEY,
        Code         nvarchar(max) NOT NULL,
        Title        nvarchar(max) NOT NULL,
        YearsOfStudy int,
        Deleted      bit           NOT NULL,
        ShortTitle   nvarchar(max),
        LevelID      int           NOT NULL,
        CONSTRAINT FK_Edu_Specialities_LevelID_Edu_SpecialityLevels FOREIGN KEY (LevelID) REFERENCES Edu_SpecialityLevels(ID)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_StudentStatuses' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_StudentStatuses (
        ID    int           NOT NULL PRIMARY KEY,
        Title nvarchar(max)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Students' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Students (
        StudentID        int NOT NULL PRIMARY KEY,
        SpecialityID     int,
        StatusID         int,
        CategoryID       int,
        Year             int NOT NULL,
        GPA              float,
        EctsGPA          float,
        EducationTypeID  int,
        GrantTypeID      int,
        AdvisorID        int,
        StudyLanguageID  int,
        AcademicStatusID int,
        IsScholarship    bit,
        NeedsDorm        bit NOT NULL,
        EntryDate        date,
        CONSTRAINT FK_Edu_Students_StudentID_Edu_Users FOREIGN KEY (StudentID) REFERENCES Edu_Users(ID),
        CONSTRAINT FK_Edu_Students_SpecialityID_Edu_Specialities FOREIGN KEY (SpecialityID) REFERENCES Edu_Specialities(ID),
        CONSTRAINT FK_Edu_Students_StatusID_Edu_StudentStatuses FOREIGN KEY (StatusID) REFERENCES Edu_StudentStatuses(ID),
        CONSTRAINT FK_Edu_Students_AdvisorID_Edu_Employees FOREIGN KEY (AdvisorID) REFERENCES Edu_Employees(ID)
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_Positions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_Positions (
        ID      int NOT NULL PRIMARY KEY,
        Title   nvarchar(max),
        Deleted bit NOT NULL
    );
END
");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Edu_EmployeePositions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE Edu_EmployeePositions (
        ID             int  NOT NULL PRIMARY KEY,
        StartedOn      date NOT NULL,
        EndedOn        date,
        Rate           float,
        IsMainPosition bit,
        OrgUnitID      int  NOT NULL,
        PositionID     int  NOT NULL,
        EmployeeID     int  NOT NULL,
        CONSTRAINT FK_Edu_EmployeePositions_OrgUnitID_Edu_OrgUnits FOREIGN KEY (OrgUnitID) REFERENCES Edu_OrgUnits(ID),
        CONSTRAINT FK_Edu_EmployeePositions_PositionID_Edu_Positions FOREIGN KEY (PositionID) REFERENCES Edu_Positions(ID),
        CONSTRAINT FK_Edu_EmployeePositions_EmployeeID_Edu_Employees FOREIGN KEY (EmployeeID) REFERENCES Edu_Employees(ID)
    );
END
");

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
