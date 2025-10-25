using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddRateLimitAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e725a73a-6ded-4337-844d-66c2be49d788", "AQAAAAIAAYagAAAAEHmzlZWotgN4vATPwGcR7M9XjdwhaNy67YYyUdRorcuTKU6ZjB9Vqx60sxBGqTQ4XA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedDate",
                table: "Posts",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsPublished",
                table: "Posts",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_IsPublished_CreatedDate",
                table: "Posts",
                columns: new[] { "IsPublished", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Title",
                table: "Posts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedDate",
                table: "Comments",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsPublished",
                table: "Comments",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_IsPublished",
                table: "Comments",
                columns: new[] { "PostId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Unique",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_UserName",
                table: "AppUsers",
                column: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedDate",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_IsPublished",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_IsPublished_CreatedDate",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Title",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedDate",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_IsPublished",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostId_IsPublished",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_Unique",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_UserName",
                table: "AppUsers");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "1672f0b2-59e2-415a-85b4-d6f6fab5822f", "AQAAAAIAAYagAAAAELXUiIGO7GfYK3MC4umGHdy8FEuUoA18YCMOlW+Zjzsk93Ajnj7G6Zlw8Cm2gEFc6w==" });
        }
    }
}
