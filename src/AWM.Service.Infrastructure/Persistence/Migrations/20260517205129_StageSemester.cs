using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StageSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Year",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Year",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Year",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Year",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropTable(
                name: "Periods",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "AcademicYears",
                schema: "Common");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AcademicYearId",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_StudentWorks_AcademicYearId",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.DropIndex(
                name: "IX_Directions_AcademicYearId",
                schema: "Thesis",
                table: "Directions");

            migrationBuilder.DropIndex(
                name: "IX_Commissions_AcademicYearId",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.CreateTable(
                name: "SemesterTypes",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterTypes", x => x.Id);
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
                name: "Semesters",
                schema: "Edu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudyYear = table.Column<int>(type: "int", nullable: false),
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
                name: "Stages",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
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
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_Semester",
                        column: x => x.SemesterId,
                        principalSchema: "Edu",
                        principalTable: "Semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stages_WorkflowStage",
                        column: x => x.WorkflowStageId,
                        principalSchema: "Common",
                        principalTable: "WorkflowStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_SemesterTypeId",
                schema: "Edu",
                table: "Semesters",
                column: "SemesterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_Active",
                schema: "Common",
                table: "Stages",
                columns: new[] { "DepartmentId", "SemesterId", "WorkflowStageId" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_SemesterId",
                schema: "Common",
                table: "Stages",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Stages_WorkflowStageId",
                schema: "Common",
                table: "Stages",
                column: "WorkflowStageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stages",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "Semesters",
                schema: "Edu");

            migrationBuilder.DropTable(
                name: "WorkflowStages",
                schema: "Common");

            migrationBuilder.DropTable(
                name: "SemesterTypes",
                schema: "Edu");

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periods",
                schema: "Common",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkflowStage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AcademicYearId",
                schema: "Thesis",
                table: "Topics",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_AcademicYearId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Directions_AcademicYearId",
                schema: "Thesis",
                table: "Directions",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_AcademicYearId",
                schema: "Defense",
                table: "Commissions",
                column: "AcademicYearId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Year",
                schema: "Defense",
                table: "Commissions",
                column: "AcademicYearId",
                principalSchema: "Common",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Year",
                schema: "Thesis",
                table: "Directions",
                column: "AcademicYearId",
                principalSchema: "Common",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Year",
                schema: "Thesis",
                table: "StudentWorks",
                column: "AcademicYearId",
                principalSchema: "Common",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Year",
                schema: "Thesis",
                table: "Topics",
                column: "AcademicYearId",
                principalSchema: "Common",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
