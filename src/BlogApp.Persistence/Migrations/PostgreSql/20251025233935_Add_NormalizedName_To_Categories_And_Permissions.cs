using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class Add_NormalizedName_To_Categories_And_Permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_Name",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_Unique",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Permissions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Categories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Mevcut verileri güncelle - Categories için NormalizedName doldur
            migrationBuilder.Sql(@"
                UPDATE ""Categories"" 
                SET ""NormalizedName"" = UPPER(""Name"") 
                WHERE ""NormalizedName"" IS NULL;
            ");

            // Mevcut verileri güncelle - Permissions için NormalizedName doldur
            migrationBuilder.Sql(@"
                UPDATE ""Permissions"" 
                SET ""NormalizedName"" = UPPER(""Name"") 
                WHERE ""NormalizedName"" IS NULL;
            ");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6,
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "f1b5535b-806c-49f2-bfae-68a80b993ed1", new DateTime(2025, 10, 25, 23, 39, 35, 342, DateTimeKind.Utc).AddTicks(1395) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "b45c1f20-7baf-4f59-b0b3-375ce095e646", new DateTime(2025, 10, 25, 23, 39, 35, 342, DateTimeKind.Utc).AddTicks(1421) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedDate" },
                values: new object[] { "81433ecf-9da1-4bba-9728-86af07fcba8d", new DateTime(2025, 10, 25, 23, 39, 35, 342, DateTimeKind.Utc).AddTicks(1435) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "AssignedDate",
                value: new DateTime(2025, 10, 25, 23, 39, 35, 342, DateTimeKind.Utc).AddTicks(3685));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "PasswordHash" },
                values: new object[] { "275d164a-817f-4115-9d4d-41fafebcc862", new DateTime(2025, 10, 25, 23, 39, 35, 342, DateTimeKind.Utc).AddTicks(7602), "AQAAAAIAAYagAAAAEAV5pYoT76cjL8rHpmQDD+29/zs8u/1iViIEuuTDUok1h++JKPGlxUkQpFPXcgKNMw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_NormalizedName_Unique",
                table: "Permissions",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_NormalizedName_Unique",
                table: "Categories",
                column: "NormalizedName",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Permissions_Name",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_NormalizedName_Unique",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_NormalizedName_Unique",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Categories");

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
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Unique",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
