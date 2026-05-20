using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropCheckConstraint(
                name: "Check_Application_Status",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_Work",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropCheckConstraint(
                name: "Check_PreDef_Attendance",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropIndex(
                name: "IX_Experts_Type",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropCheckConstraint(
                name: "Check_Expert_Type",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Edu",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CheckType",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropColumn(
                name: "AttendanceStatus",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropColumn(
                name: "ExpertiseType",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "RoleInCommission",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropColumn(
                name: "AttachmentType",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "Thesis",
                table: "WorkParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "Thesis",
                table: "TopicApplications",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "Edu",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CheckTypeId",
                schema: "Thesis",
                table: "QualityChecks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "CheckTypeId",
                schema: "Thesis",
                table: "Experts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommissionTypeId",
                schema: "Defense",
                table: "Commissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommissionRoleId",
                schema: "Defense",
                table: "CommissionMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttachmentTypeId",
                schema: "Thesis",
                table: "Attachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApplicationStatuses",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentTypes",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceStatuses",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckTypes",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionRoles",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTypes",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantRoles",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantRoles", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_WorkParticipants_RoleId",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "RoleId");

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
                name: "IX_Students_StatusId",
                schema: "Edu",
                table: "Students",
                column: "StatusId");

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
                name: "IX_PreDefenseAttempts_AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts",
                column: "AttendanceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_CheckTypeId",
                schema: "Thesis",
                table: "Experts",
                column: "CheckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_Type",
                schema: "Thesis",
                table: "Experts",
                columns: new[] { "DepartmentId", "CheckTypeId" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CommissionTypeId",
                schema: "Defense",
                table: "Commissions",
                column: "CommissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionMembers_CommissionRoleId",
                schema: "Defense",
                table: "CommissionMembers",
                column: "CommissionRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentTypeId",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attach_Type",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId",
                principalSchema: "Thesis",
                principalTable: "AttachmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommMembers_Role",
                schema: "Defense",
                table: "CommissionMembers",
                column: "CommissionRoleId",
                principalSchema: "Defense",
                principalTable: "CommissionRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Commissions_Type",
                schema: "Defense",
                table: "Commissions",
                column: "CommissionTypeId",
                principalSchema: "Defense",
                principalTable: "CommissionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Experts_CheckType",
                schema: "Thesis",
                table: "Experts",
                column: "CheckTypeId",
                principalSchema: "Thesis",
                principalTable: "CheckTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PreDef_AttendanceStatus",
                schema: "Defense",
                table: "PreDefenseAttempts",
                column: "AttendanceStatusId",
                principalSchema: "Defense",
                principalTable: "AttendanceStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QChecks_Type",
                schema: "Thesis",
                table: "QualityChecks",
                column: "CheckTypeId",
                principalSchema: "Thesis",
                principalTable: "CheckTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Status",
                schema: "Edu",
                table: "Students",
                column: "StatusId",
                principalSchema: "Edu",
                principalTable: "StudentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications",
                column: "StatusId",
                principalSchema: "Thesis",
                principalTable: "ApplicationStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Role",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "RoleId",
                principalSchema: "Thesis",
                principalTable: "ParticipantRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attach_Type",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommMembers_Role",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Type",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Experts_CheckType",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropForeignKey(
                name: "FK_PreDef_AttendanceStatus",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecks_Type",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Status",
                schema: "Edu",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Role",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropTable(
                name: "ApplicationStatuses",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "AttachmentTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "AttendanceStatuses",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "CheckTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "CommissionRoles",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "CommissionTypes",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "ParticipantRoles",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "StudentStatuses",
                schema: "Edu");

            migrationBuilder.DropIndex(
                name: "IX_WorkParticipants_RoleId",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Status",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_Student",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropIndex(
                name: "IX_Students_StatusId",
                schema: "Edu",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_CheckTypeId",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_Work",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "IX_PreDefenseAttempts_AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropIndex(
                name: "IX_Experts_CheckTypeId",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropIndex(
                name: "IX_Experts_Type",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropIndex(
                name: "IX_Commissions_CommissionTypeId",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropIndex(
                name: "IX_CommissionMembers_CommissionRoleId",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_AttachmentTypeId",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "Thesis",
                table: "TopicApplications");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "Edu",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CheckTypeId",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropColumn(
                name: "AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropColumn(
                name: "CheckTypeId",
                schema: "Thesis",
                table: "Experts");

            migrationBuilder.DropColumn(
                name: "CommissionTypeId",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "CommissionRoleId",
                schema: "Defense",
                table: "CommissionMembers");

            migrationBuilder.DropColumn(
                name: "AttachmentTypeId",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "Thesis",
                table: "WorkParticipants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Thesis",
                table: "TopicApplications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Submitted");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Edu",
                table: "Students",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CheckType",
                schema: "Thesis",
                table: "QualityChecks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceStatus",
                schema: "Defense",
                table: "PreDefenseAttempts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Attended");

            migrationBuilder.AddColumn<string>(
                name: "ExpertiseType",
                schema: "Thesis",
                table: "Experts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CommissionType",
                schema: "Defense",
                table: "Commissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RoleInCommission",
                schema: "Defense",
                table: "CommissionMembers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                schema: "Thesis",
                table: "Attachments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddCheckConstraint(
                name: "Check_Application_Status",
                schema: "Thesis",
                table: "TopicApplications",
                sql: "[Status] IN ('Submitted', 'Accepted', 'Rejected', 'Withdrawn')");

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_Work",
                schema: "Thesis",
                table: "QualityChecks",
                columns: new[] { "WorkId", "CheckType", "AttemptNumber" });

            migrationBuilder.AddCheckConstraint(
                name: "Check_PreDef_Attendance",
                schema: "Defense",
                table: "PreDefenseAttempts",
                sql: "[AttendanceStatus] IN ('Attended', 'Absent', 'Excused')");

            migrationBuilder.CreateIndex(
                name: "IX_Experts_Type",
                schema: "Thesis",
                table: "Experts",
                columns: new[] { "DepartmentId", "ExpertiseType" },
                filter: "[IsActive] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "Check_Expert_Type",
                schema: "Thesis",
                table: "Experts",
                sql: "[ExpertiseType] IN ('NormControl', 'SoftwareCheck', 'AntiPlagiarism')");
        }
    }
}
