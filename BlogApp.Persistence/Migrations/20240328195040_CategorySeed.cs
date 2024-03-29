using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategorySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d063598f-d9eb-482e-8e70-16bdab98f4b0", "AQAAAAIAAYagAAAAEMlrp/Z/dJ52OKdLaM8SCrZr44yfzseu2Ovzz7hPFqw7Cbw6XTQIkxJeebYiOpilIg==", "31f99456-7484-4a0a-a794-5375cf9901ce" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "IsDeleted", "Name", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5255), null, false, "ASP .NET Core", null, null },
                    { 2, 1, new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5270), null, false, "Entity Framework Core", null, null },
                    { 3, 1, new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5271), null, false, "Docker", null, null },
                    { 4, 1, new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5272), null, false, "RabbitMQ", null, null },
                    { 5, 1, new DateTime(2024, 3, 28, 22, 50, 40, 453, DateTimeKind.Local).AddTicks(5273), null, false, "Redis", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61f853c4-6114-4190-aa16-aa34ab168165", "AQAAAAIAAYagAAAAEM8qtw3WHy46YCWaFRt1yitKqCADsGSU5g6outt7dqSp3X+mgLQVt/RHwrLvXNte0g==", "3fbadfc2-2e17-4c69-98bc-5a666fcc43d2" });
        }
    }
}
