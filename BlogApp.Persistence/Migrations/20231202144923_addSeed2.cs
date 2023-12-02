using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addSeed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e72982ed-16aa-4e9b-bcde-4470c86a3389", "AQAAAAIAAYagAAAAEAXiN1jPe7uoVWibOxGcFWb/GKxLGGOZzEQ2mDZEJVWzx4+mbx7Dm+YRVrWUuisltA==", "266b0a5e-8167-4b70-907c-cb7ee80a48be" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a926992c-ae8c-405a-9168-875ebb728b6e", "AQAAAAIAAYagAAAAEP3bIHxusAWzVaVzeevxzPk6lzMlfW9n33hQJdlCmR/AV0qW9iJrxDFZ6LuxLzO97w==", null });
        }
    }
}
