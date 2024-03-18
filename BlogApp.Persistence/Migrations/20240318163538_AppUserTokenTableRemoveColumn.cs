using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppUserTokenTableRemoveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "AppUserTokens");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61f853c4-6114-4190-aa16-aa34ab168165", "AQAAAAIAAYagAAAAEM8qtw3WHy46YCWaFRt1yitKqCADsGSU5g6outt7dqSp3X+mgLQVt/RHwrLvXNte0g==", "3fbadfc2-2e17-4c69-98bc-5a666fcc43d2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "AppUserTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23e7c328-3e83-42ae-841d-59607ff7c8de", "AQAAAAIAAYagAAAAEJn21yRzGG5JDUWBqbnCodHL7zLqfJa0onuY1sJcCje/tM0jSX4WsWmvisx9wY6k8A==", "4dc841fb-4f03-4bd9-b5d5-9e3121b9737b" });
        }
    }
}
