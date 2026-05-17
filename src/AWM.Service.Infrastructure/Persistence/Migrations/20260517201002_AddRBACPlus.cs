using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AWM.Service.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRBACPlus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoleAssignments",
                schema: "Auth");

            migrationBuilder.CreateTable(
                name: "RoleAccesses",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleActionTypes",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperations",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameRu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameKz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOperations_RoleOperations_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Auth",
                        principalTable: "RoleOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessHistories",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_RoleAccesses_RoleAccessId",
                        column: x => x.RoleAccessId,
                        principalSchema: "Auth",
                        principalTable: "RoleAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleOperationActions",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleAccessId = table.Column<int>(type: "int", nullable: false),
                    RoleOperationId = table.Column<int>(type: "int", nullable: false),
                    RoleActionTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleOperationActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleAccesses_RoleAccessId",
                        column: x => x.RoleAccessId,
                        principalSchema: "Auth",
                        principalTable: "RoleAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleActionTypes_RoleActionTypeId",
                        column: x => x.RoleActionTypeId,
                        principalSchema: "Auth",
                        principalTable: "RoleActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleOperationActions_RoleOperations_RoleOperationId",
                        column: x => x.RoleOperationId,
                        principalSchema: "Auth",
                        principalTable: "RoleOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_RoleAccess_Code",
                schema: "Auth",
                table: "RoleAccesses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_RoleActionType_Code",
                schema: "Auth",
                table: "RoleActionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationActions_RoleActionTypeId",
                schema: "Auth",
                table: "RoleOperationActions",
                column: "RoleActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperationActions_RoleOperationId",
                schema: "Auth",
                table: "RoleOperationActions",
                column: "RoleOperationId");

            migrationBuilder.CreateIndex(
                name: "UQ_RoleOperationAction",
                schema: "Auth",
                table: "RoleOperationActions",
                columns: new[] { "RoleAccessId", "RoleOperationId", "RoleActionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleOperations_Tree",
                schema: "Auth",
                table: "RoleOperations",
                columns: new[] { "ParentId", "OrderBy" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_RoleAccessId",
                schema: "Auth",
                table: "UserAccesses",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserAccess",
                schema: "Auth",
                table: "UserAccesses",
                columns: new[] { "UserId", "RoleAccessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessHistory_Role",
                schema: "Auth",
                table: "UserAccessHistories",
                column: "RoleAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessHistory_User",
                schema: "Auth",
                table: "UserAccessHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleOperationActions",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "UserAccesses",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "UserAccessHistories",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleActionTypes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleOperations",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleAccesses",
                schema: "Auth");

            migrationBuilder.CreateTable(
                name: "UserRoleAssignments",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    InstituteId = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_URA_Dept",
                        column: x => x.DepartmentId,
                        principalSchema: "Org",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_Institute",
                        column: x => x.InstituteId,
                        principalSchema: "Org",
                        principalTable: "Institutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_Role",
                        column: x => x.RoleId,
                        principalSchema: "Auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_URA_User",
                        column: x => x.UserId,
                        principalSchema: "Auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_URA_Year",
                        column: x => x.AcademicYearId,
                        principalSchema: "Common",
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_URA_UserCtx",
                schema: "Auth",
                table: "UserRoleAssignments",
                columns: new[] { "UserId", "DepartmentId" },
                filter: "[DepartmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_AcademicYearId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_DepartmentId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_InstituteId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAssignments_RoleId",
                schema: "Auth",
                table: "UserRoleAssignments",
                column: "RoleId");
        }
    }
}
