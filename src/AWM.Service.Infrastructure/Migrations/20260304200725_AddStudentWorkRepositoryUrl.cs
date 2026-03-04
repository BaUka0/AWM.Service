using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentWorkRepositoryUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsReconciliationStarted",
                schema: "Defense",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "IsReconciliationStarted",
                schema: "Defense",
                table: "Schedules");
        }
    }
}
