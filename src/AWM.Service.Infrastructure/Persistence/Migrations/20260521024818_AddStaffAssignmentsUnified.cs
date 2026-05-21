using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAssignmentsUnified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commissions_Type",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Member",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_PreDef_AttendanceStatus",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropForeignKey(
                name: "FK_QChecks_Expert",
                schema: "Thesis",
                table: "QualityChecks");

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
                name: "AttendanceStatuses",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "CommissionMembers",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "CommissionTypes",
                schema: "Defense");

            migrationBuilder.DropTable(
                name: "Experts",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "ParticipantRoles",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "CommissionRoles",
                schema: "Defense");

            migrationBuilder.DropIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes");

            migrationBuilder.DropIndex(
                name: "IX_WorkParticipants_RoleId",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropIndex(
                name: "UQ_SupReview_Work_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews");

            migrationBuilder.DropIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States");

            migrationBuilder.DropIndex(
                name: "UQ_Schedule_Commission_Work",
                schema: "Defense",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_AssignedExpertId",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "UQ_Protocol_Schedule",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "IX_PreDefenseAttempts_AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts");

            migrationBuilder.DropIndex(
                name: "UQ_Template_Event",
                schema: "Common",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Grades_MemberId",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "UQ_Grade_Schedule_Member_Criteria",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Commissions_CommissionTypeId",
                schema: "Defense",
                table: "Commissions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "Thesis",
                table: "WorkParticipants");

            migrationBuilder.DropColumn(
                name: "RepositoryUrl",
                schema: "Thesis",
                table: "StudentWorks")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "MemberId",
                schema: "Defense",
                table: "Grades")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "MetadataJson",
                schema: "Thesis",
                table: "StudentWorks",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<long>(
                name: "AssignmentId",
                schema: "Defense",
                table: "Grades",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Thesis",
                table: "CheckTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "Thesis",
                table: "CheckTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasNumericResult",
                schema: "Thesis",
                table: "CheckTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Thesis",
                table: "AttachmentTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "Thesis",
                table: "AttachmentTypes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_SupReview_Work_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews",
                columns: new[] { "WorkId", "SupervisorId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States",
                columns: new[] { "WorkTypeId", "SystemName" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_Schedule_Commission_Work",
                schema: "Defense",
                table: "Schedules",
                columns: new[] { "CommissionId", "WorkId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_Protocol_Schedule",
                schema: "Defense",
                table: "Protocols",
                column: "ScheduleId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_Template_Event",
                schema: "Common",
                table: "NotificationTemplates",
                column: "EventType",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_AssignmentId",
                schema: "Defense",
                table: "Grades",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "UQ_Grade_Schedule_Assignment_Criteria",
                schema: "Defense",
                table: "Grades",
                columns: new[] { "ScheduleId", "AssignmentId", "CriteriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AttachmentTypes_AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments",
                column: "AttachmentTypeId1",
                principalSchema: "Thesis",
                principalTable: "AttachmentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Assignment",
                schema: "Defense",
                table: "Grades",
                column: "AssignmentId",
                principalSchema: "Common",
                principalTable: "StaffAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AttachmentTypes_AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Assignment",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropTable(
                name: "SpecialityCheckTypes",
                schema: "Thesis");

            migrationBuilder.DropTable(
                name: "StaffAssignments",
                schema: "Common");

            migrationBuilder.DropIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes");

            migrationBuilder.DropIndex(
                name: "UQ_SupReview_Work_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews");

            migrationBuilder.DropIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States");

            migrationBuilder.DropIndex(
                name: "UQ_Schedule_Commission_Work",
                schema: "Defense",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "UQ_Protocol_Schedule",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropIndex(
                name: "UQ_Template_Event",
                schema: "Common",
                table: "NotificationTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Grades_AssignmentId",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "UQ_Grade_Schedule_Assignment_Criteria",
                schema: "Defense",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "MetadataJson",
                schema: "Thesis",
                table: "StudentWorks")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                schema: "Defense",
                table: "Grades")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "Thesis",
                table: "CheckTypes");

            migrationBuilder.DropColumn(
                name: "HasNumericResult",
                schema: "Thesis",
                table: "CheckTypes");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "Thesis",
                table: "AttachmentTypes");

            migrationBuilder.DropColumn(
                name: "AttachmentTypeId1",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                schema: "Thesis",
                table: "WorkParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RepositoryUrl",
                schema: "Thesis",
                table: "StudentWorks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StudentWorksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                schema: "Defense",
                table: "Grades",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "GradesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Defense")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Thesis",
                table: "CheckTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "Thesis",
                table: "AttachmentTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

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
                name: "Experts",
                schema: "Thesis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Experts_CheckType",
                        column: x => x.CheckTypeId,
                        principalSchema: "Thesis",
                        principalTable: "CheckTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experts_Dept",
                        column: x => x.DepartmentId,
                        principalTable: "Edu_OrgUnits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Experts_User",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                name: "CommissionMembers",
                schema: "Defense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    CommissionRoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                        name: "FK_CommMembers_Role",
                        column: x => x.CommissionRoleId,
                        principalSchema: "Defense",
                        principalTable: "CommissionRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommMembers_User",
                        column: x => x.UserId,
                        principalTable: "Edu_Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_WorkType_Name",
                schema: "Wf",
                table: "WorkTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkParticipants_RoleId",
                schema: "Thesis",
                table: "WorkParticipants",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ_SupReview_Work_Supervisor",
                schema: "Thesis",
                table: "SupervisorReviews",
                columns: new[] { "WorkId", "SupervisorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_State_Type_Name",
                schema: "Wf",
                table: "States",
                columns: new[] { "WorkTypeId", "SystemName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Schedule_Commission_Work",
                schema: "Defense",
                table: "Schedules",
                columns: new[] { "CommissionId", "WorkId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_AssignedExpertId",
                schema: "Thesis",
                table: "QualityChecks",
                column: "AssignedExpertId");

            migrationBuilder.CreateIndex(
                name: "UQ_Protocol_Schedule",
                schema: "Defense",
                table: "Protocols",
                column: "ScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreDefenseAttempts_AttendanceStatusId",
                schema: "Defense",
                table: "PreDefenseAttempts",
                column: "AttendanceStatusId");

            migrationBuilder.CreateIndex(
                name: "UQ_Template_Event",
                schema: "Common",
                table: "NotificationTemplates",
                column: "EventType",
                unique: true);

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
                name: "IX_Experts_UserId",
                schema: "Thesis",
                table: "Experts",
                column: "UserId");

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
                name: "FK_Grades_Member",
                schema: "Defense",
                table: "Grades",
                column: "MemberId",
                principalSchema: "Defense",
                principalTable: "CommissionMembers",
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
                name: "FK_QChecks_Expert",
                schema: "Thesis",
                table: "QualityChecks",
                column: "AssignedExpertId",
                principalSchema: "Thesis",
                principalTable: "Experts",
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
    }
}
