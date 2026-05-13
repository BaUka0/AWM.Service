using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrgCatalogsCRUD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "Edu",
                table: "DegreeLevels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                schema: "Edu",
                table: "DegreeLevels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "Edu",
                table: "DegreeLevels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "Edu",
                table: "DegreeLevels");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Edu",
                table: "DegreeLevels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "Edu",
                table: "DegreeLevels");
        }
    }
}
