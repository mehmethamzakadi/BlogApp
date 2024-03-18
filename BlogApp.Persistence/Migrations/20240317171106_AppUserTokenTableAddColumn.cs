using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppUserTokenTableAddColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "AppUserTokens");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3e5c038-cff6-44e8-ba15-aa4a284c48df", "AQAAAAIAAYagAAAAEB5pDWAjj4BQsfGHNnHiKQz7TStMm4Jr9P1eiHi1Cz2eqlRinx4QQdTORHzhp+5MpQ==", "343a2a70-e26c-41af-ae7c-051d002210c5" });
        }
    }
}
