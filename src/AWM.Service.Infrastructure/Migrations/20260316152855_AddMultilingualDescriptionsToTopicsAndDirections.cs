using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualDescriptionsToTopicsAndDirections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Thesis",
                table: "Topics",
                newName: "DescriptionRu")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "Thesis",
                table: "Directions",
                newName: "DescriptionRu")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                schema: "Thesis",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionKz",
                schema: "Thesis",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                schema: "Thesis",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionKz",
                schema: "Thesis",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: true)
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "DescriptionKz",
                schema: "Thesis",
                table: "Topics")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                schema: "Thesis",
                table: "Directions")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.DropColumn(
                name: "DescriptionKz",
                schema: "Thesis",
                table: "Directions")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "DescriptionRu",
                schema: "Thesis",
                table: "Topics",
                newName: "Description")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TopicsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");

            migrationBuilder.RenameColumn(
                name: "DescriptionRu",
                schema: "Thesis",
                table: "Directions",
                newName: "Description")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "DirectionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "Thesis")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEndTime")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStartTime");
        }
    }
}
