using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddImageUrlToBookshelfItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "BookshelfItems",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                columns: new[] { "Description", "Module", "Name", "NormalizedName", "Type" },
                values: new object[] { "Sisteme görsel yükleme yetkisi.", "Media", "Media.Upload", "MEDIA.UPLOAD", "Upload" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "Description", "IsDeleted", "Module", "Name", "NormalizedName", "Type", "UpdatedById", "UpdatedDate" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Aktivite loglarını görüntüleme yetkisi.", false, "ActivityLogs", "ActivityLogs.View", "ACTIVITYLOGS.VIEW", "View", null, null });


            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "GrantedAt" },
                values: new object[] { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000036"));

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "BookshelfItems");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"),
                columns: new[] { "Description", "Module", "Name", "NormalizedName", "Type" },
                values: new object[] { "Aktivite loglarını görüntüleme yetkisi.", "ActivityLogs", "ActivityLogs.View", "ACTIVITYLOGS.VIEW", "View" });

        }
    }
}
