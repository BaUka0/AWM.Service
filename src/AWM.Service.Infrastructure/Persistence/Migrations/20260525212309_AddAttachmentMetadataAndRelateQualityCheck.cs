using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentMetadataAndRelateQualityCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "IsSubmittedForApproval",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Thesis",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<long>(
                name: "AttachmentId",
                schema: "Thesis",
                table: "QualityChecks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                schema: "Defense",
                table: "Protocols",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                schema: "Thesis",
                table: "Attachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                schema: "Thesis",
                table: "Attachments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics",
                columns: new[] { "OrgUnitId", "SemesterId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_AttachmentId",
                schema: "Thesis",
                table: "QualityChecks",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QualityCheck_Attachment",
                schema: "Thesis",
                table: "QualityChecks",
                column: "AttachmentId",
                principalSchema: "Thesis",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QualityCheck_Attachment",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_QualityChecks_AttachmentId",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                schema: "Thesis",
                table: "QualityChecks");

            migrationBuilder.DropColumn(
                name: "Comments",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "ContentType",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                schema: "Thesis",
                table: "Attachments");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                schema: "Thesis",
                table: "Topics",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                schema: "Thesis",
                table: "Topics",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                schema: "Thesis",
                table: "Topics",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<bool>(
                name: "IsSubmittedForApproval",
                schema: "Thesis",
                table: "Topics",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                schema: "Thesis",
                table: "QualityChecks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Filter",
                schema: "Thesis",
                table: "Topics",
                columns: new[] { "OrgUnitId", "SemesterId", "IsApproved" });
        }
    }
}
