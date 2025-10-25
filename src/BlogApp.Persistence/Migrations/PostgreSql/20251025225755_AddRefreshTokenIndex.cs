using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddRefreshTokenIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "459226f9-2b10-4bd4-960f-af9e91305fb0", new DateTime(2025, 10, 25, 22, 57, 55, 77, DateTimeKind.Utc).AddTicks(6075) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "be8e790b-ac87-4a98-8097-783d90c99876", new DateTime(2025, 10, 25, 22, 57, 55, 77, DateTimeKind.Utc).AddTicks(6108) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "3958ac9a-448d-412d-bdd6-95d4dc17a0dd", new DateTime(2025, 10, 25, 22, 57, 55, 77, DateTimeKind.Utc).AddTicks(6112) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedDate",
                value: new DateTime(2025, 10, 25, 22, 57, 55, 77, DateTimeKind.Utc).AddTicks(8358));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash" },
                values: new object[] { "bf2d4755-be1d-40dd-b939-d891c6e1af51", new DateTime(2025, 10, 25, 22, 57, 55, 78, DateTimeKind.Utc).AddTicks(2940), "AQAAAAIAAYagAAAAEJVNNWsyqCt2PaHbzgdxryfQ6AnXAlwiEd4Qn2BiZmgZ6vgEpXRBXtp2Sefx83LFFw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshToken",
                table: "Users",
                column: "RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_RefreshToken",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "21d9aa3d-3352-4ccd-a099-9c0be98eac2a", new DateTime(2025, 10, 25, 22, 47, 24, 537, DateTimeKind.Utc).AddTicks(340) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "f2f2c28b-c023-458c-b44a-769cae779914", new DateTime(2025, 10, 25, 22, 47, 24, 537, DateTimeKind.Utc).AddTicks(368) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "d692f597-494c-4efc-a311-530b561c46c3", new DateTime(2025, 10, 25, 22, 47, 24, 537, DateTimeKind.Utc).AddTicks(372) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedDate",
                value: new DateTime(2025, 10, 25, 22, 47, 24, 537, DateTimeKind.Utc).AddTicks(2507));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash" },
                values: new object[] { "f79b872a-e9eb-450b-9849-7a01cfcd9e85", new DateTime(2025, 10, 25, 22, 47, 24, 537, DateTimeKind.Utc).AddTicks(6452), "AQAAAAIAAYagAAAAEMKYUqcGebXAfdxBIIPg80gWWaeCUtEVUMrYJ3SJarM8wV02ekruSXTYxLhXYYzbUg==" });
        }
    }
}
