using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddBookshelfModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookshelfItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Publisher = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PageCount = table.Column<int>(type: "integer", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ReadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookshelfItems", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                columns: new[] { "Description", "Module", "Name", "NormalizedName", "Type" },
                values: new object[] { "Yeni kitap kaydı oluşturma yetkisi.", "Bookshelf", "Bookshelf.Create", "BOOKSHELF.CREATE", "Create" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "Description", "IsDeleted", "Module", "Name", "NormalizedName", "Type", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kayıtlarını görüntüleme yetkisi.", false, "Bookshelf", "Bookshelf.Read", "BOOKSHELF.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kayıtlarını güncelleme yetkisi.", false, "Bookshelf", "Bookshelf.Update", "BOOKSHELF.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kaydı silme yetkisi.", false, "Bookshelf", "Bookshelf.Delete", "BOOKSHELF.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm kitap kayıtlarını görüntüleme yetkisi.", false, "Bookshelf", "Bookshelf.ViewAll", "BOOKSHELF.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Aktivite loglarını görüntüleme yetkisi.", false, "ActivityLogs", "ActivityLogs.View", "ACTIVITYLOGS.VIEW", "View", null, null }
                });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                column: "Thumbnail",
                value: "https://miro.medium.com/v2/resize:fit:1286/format:webp/1*chhJLW0ApPDHqmVPRBBUtQ.png");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                column: "Thumbnail",
                value: "https://miro.medium.com/0*V56TEDMUsms9XLBY.jpg");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                column: "Thumbnail",
                value: "https://miro.medium.com/v2/resize:fit:4800/format:webp/1*9P6wnky3C9xMwaBAElALLQ.png");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"),
                column: "Thumbnail",
                value: "https://miro.medium.com/v2/resize:fit:1400/format:webp/1*zHc9d823Uol9SSj8s_uBug.png");

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "GrantedAt" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfItems_CreatedDate",
                table: "BookshelfItems",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfItems_IsRead",
                table: "BookshelfItems",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfItems_ReadDate",
                table: "BookshelfItems",
                column: "ReadDate");

            migrationBuilder.CreateIndex(
                name: "IX_BookshelfItems_Title",
                table: "BookshelfItems",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookshelfItems");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000032"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000033"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000034"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000035"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000030"),
                columns: new[] { "Description", "Module", "Name", "NormalizedName", "Type" },
                values: new object[] { "Aktivite loglarını görüntüleme yetkisi.", "ActivityLogs", "ActivityLogs.View", "ACTIVITYLOGS.VIEW", "View" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"),
                column: "Thumbnail",
                value: "/media/posts/dotnet-minimal-api-observability.png");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"),
                column: "Thumbnail",
                value: "/media/posts/ef-core-multi-tenant-tips.png");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"),
                column: "Thumbnail",
                value: "/media/posts/gitops-cd-pipelines.png");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"),
                column: "Thumbnail",
                value: "/media/posts/opentelemetry-layered-observability.png");
        }
    }
}
