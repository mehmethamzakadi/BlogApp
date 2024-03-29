using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategorySeedEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8d9ae91a-e7c0-43c2-9fb4-1e7c46d73243", "AQAAAAIAAYagAAAAEC14nlHpTEloKJCNgwoSuG50FU4c4Tqri+FuVivLNiR/MKFKBiuc1vZX2dwUIdfhvQ==", "3b3fd2c3-8fd1-4334-bd8e-4508facdca00" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9147));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9163));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9164));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9165));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9166));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "IsDeleted", "Name", "UpdatedById", "UpdatedDate" },
                values: new object[] { 6, 1, new DateTime(2024, 3, 28, 22, 54, 35, 679, DateTimeKind.Local).AddTicks(9168), null, false, "Clean Architecture", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d063598f-d9eb-482e-8e70-16bdab98f4b0", "AQAAAAIAAYagAAAAEMlrp/Z/dJ52OKdLaM8SCrZr44yfzseu2Ovzz7hPFqw7Cbw6XTQIkxJeebYiOpilIg==", "31f99456-7484-4a0a-a794-5375cf9901ce" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5255));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5270));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5271));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5272));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5273));
        }
    }
}
