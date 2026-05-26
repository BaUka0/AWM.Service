using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpecialityCheckTypeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Speciality_CheckType",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialityId",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPassValue",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrgUnitId",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialityCheckTypes_SpecialityId",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                column: "SpecialityId");

            migrationBuilder.CreateIndex(
                name: "UQ_OrgUnit_Speciality_CheckType",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                columns: new[] { "OrgUnitId", "SpecialityId", "CheckTypeId" },
                unique: true,
                filter: "[SpecialityId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecChecks_OrgUnit",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                column: "OrgUnitId",
                principalTable: "Edu_OrgUnits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecChecks_OrgUnit",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.DropIndex(
                name: "IX_SpecialityCheckTypes_SpecialityId",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.DropIndex(
                name: "UQ_OrgUnit_Speciality_CheckType",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.DropColumn(
                name: "MinimumPassValue",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.DropColumn(
                name: "OrgUnitId",
                schema: "Thesis",
                table: "SpecialityCheckTypes");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialityId",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Speciality_CheckType",
                schema: "Thesis",
                table: "SpecialityCheckTypes",
                columns: new[] { "SpecialityId", "CheckTypeId" },
                unique: true);
        }
    }
}
