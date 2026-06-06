using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintOnStudentWorksTopicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentWorks_TopicId",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.CreateIndex(
                name: "UQ_Works_Topic",
                schema: "Thesis",
                table: "StudentWorks",
                column: "TopicId",
                unique: true,
                filter: "[TopicId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Works_Topic",
                schema: "Thesis",
                table: "StudentWorks");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorks_TopicId",
                schema: "Thesis",
                table: "StudentWorks",
                column: "TopicId");
        }
    }
}
