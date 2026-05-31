using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefenseStageTypeAndProtocolDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DecisionType",
                schema: "Defense",
                table: "Protocols",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReadinessPercent",
                schema: "Defense",
                table: "Protocols",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefenseStageType",
                schema: "Defense",
                table: "EvaluationCriteria",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                schema: "Defense",
                table: "EvaluationCriteria",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecisionType",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "ReadinessPercent",
                schema: "Defense",
                table: "Protocols");

            migrationBuilder.DropColumn(
                name: "DefenseStageType",
                schema: "Defense",
                table: "EvaluationCriteria");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                schema: "Defense",
                table: "EvaluationCriteria");
        }
    }
}
