using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class Init : Migration
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
                    ImageUrl = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookshelfItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NextRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Jti = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReplacedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, computedColumnSql: "UPPER(\"UserName\")", stored: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, computedColumnSql: "UPPER(\"Email\")", stored: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    PasswordResetToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Thumbnail = table.Column<string>(type: "text", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CommentOwnerMail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "IsDeleted", "Name", "NormalizedName", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "ASP.NET Core", "ASP.NET CORE", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "Entity Framework Core", "ENTITY FRAMEWORK CORE", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "Cloud Native DevOps", "CLOUD NATIVE DEVOPS", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "Event-Driven Messaging", "EVENT-DRIVEN MESSAGING", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "Observability & Monitoring", "OBSERVABILITY & MONITORING", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, false, "Clean Architecture", "CLEAN ARCHITECTURE", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "DeletedDate", "Description", "IsDeleted", "Module", "Name", "NormalizedName", "Type", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yönetim paneli dashboard ekranına erişim.", false, "Dashboard", "Dashboard.View", "DASHBOARD.VIEW", "View", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni kullanıcı oluşturma yetkisi.", false, "Users", "Users.Create", "USERS.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kullanıcı bilgilerini görüntüleme yetkisi.", false, "Users", "Users.Read", "USERS.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kullanıcı bilgilerini güncelleme yetkisi.", false, "Users", "Users.Update", "USERS.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kullanıcı silme yetkisi.", false, "Users", "Users.Delete", "USERS.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm kullanıcıları görüntüleme yetkisi.", false, "Users", "Users.ViewAll", "USERS.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni rol oluşturma yetkisi.", false, "Roles", "Roles.Create", "ROLES.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Rolleri görüntüleme yetkisi.", false, "Roles", "Roles.Read", "ROLES.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Rolleri güncelleme yetkisi.", false, "Roles", "Roles.Update", "ROLES.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000010"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Rol silme yetkisi.", false, "Roles", "Roles.Delete", "ROLES.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000011"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm rolleri görüntüleme yetkisi.", false, "Roles", "Roles.ViewAll", "ROLES.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000012"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Rollere yetki atama yetkisi.", false, "Roles", "Roles.AssignPermissions", "ROLES.ASSIGNPERMISSIONS", "AssignPermissions", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000013"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni blog yazısı oluşturma yetkisi.", false, "Posts", "Posts.Create", "POSTS.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Blog yazılarını görüntüleme yetkisi.", false, "Posts", "Posts.Read", "POSTS.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Blog yazılarını güncelleme yetkisi.", false, "Posts", "Posts.Update", "POSTS.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000016"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Blog yazısı silme yetkisi.", false, "Posts", "Posts.Delete", "POSTS.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000017"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm blog yazılarını görüntüleme yetkisi.", false, "Posts", "Posts.ViewAll", "POSTS.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000018"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Blog yazısı yayınlama yetkisi.", false, "Posts", "Posts.Publish", "POSTS.PUBLISH", "Publish", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000019"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni kategori oluşturma yetkisi.", false, "Categories", "Categories.Create", "CATEGORIES.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kategorileri görüntüleme yetkisi.", false, "Categories", "Categories.Read", "CATEGORIES.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000021"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kategorileri güncelleme yetkisi.", false, "Categories", "Categories.Update", "CATEGORIES.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000022"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kategori silme yetkisi.", false, "Categories", "Categories.Delete", "CATEGORIES.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000023"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm kategorileri görüntüleme yetkisi.", false, "Categories", "Categories.ViewAll", "CATEGORIES.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000024"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni yorum oluşturma yetkisi.", false, "Comments", "Comments.Create", "COMMENTS.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yorumları görüntüleme yetkisi.", false, "Comments", "Comments.Read", "COMMENTS.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000026"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yorumları güncelleme yetkisi.", false, "Comments", "Comments.Update", "COMMENTS.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yorum silme yetkisi.", false, "Comments", "Comments.Delete", "COMMENTS.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000028"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm yorumları görüntüleme yetkisi.", false, "Comments", "Comments.ViewAll", "COMMENTS.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yorum moderasyonu yapma yetkisi.", false, "Comments", "Comments.Moderate", "COMMENTS.MODERATE", "Moderate", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000030"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yeni kitap kaydı oluşturma yetkisi.", false, "Bookshelf", "Bookshelf.Create", "BOOKSHELF.CREATE", "Create", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kayıtlarını görüntüleme yetkisi.", false, "Bookshelf", "Bookshelf.Read", "BOOKSHELF.READ", "Read", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kayıtlarını güncelleme yetkisi.", false, "Bookshelf", "Bookshelf.Update", "BOOKSHELF.UPDATE", "Update", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kitap kaydı silme yetkisi.", false, "Bookshelf", "Bookshelf.Delete", "BOOKSHELF.DELETE", "Delete", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm kitap kayıtlarını görüntüleme yetkisi.", false, "Bookshelf", "Bookshelf.ViewAll", "BOOKSHELF.VIEWALL", "ViewAll", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Sisteme görsel yükleme yetkisi.", false, "Media", "Media.Upload", "MEDIA.UPLOAD", "Upload", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Aktivite loglarını görüntüleme yetkisi.", false, "ActivityLogs", "ActivityLogs.View", "ACTIVITYLOGS.VIEW", "View", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedById", "CreatedDate", "DeletedDate", "Description", "IsDeleted", "Name", "NormalizedName", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "11111111-1111-1111-1111-111111111111", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Tüm sistemi yönetebilen ve yetki atayabilen tam yetkili rol.", false, "Admin", "ADMIN", null, null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "22222222-2222-2222-2222-222222222222", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Kendi içeriklerini oluşturup yönetebilen topluluk yazarı rolü.", false, "User", "USER", null, null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "33333333-3333-3333-3333-333333333333", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yorumları ve topluluk etkileşimlerini yöneten rol.", false, "Moderator", "MODERATOR", null, null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "44444444-4444-4444-4444-444444444444", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, "Yayın akışını yöneten, içerikleri yayımlayan ve kategorileri düzenleyen rol.", false, "Editor", "EDITOR", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedById", "CreatedDate", "DeletedDate", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UpdatedById", "UpdatedDate", "Email", "UserName" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), 0, "55555555-5555-5555-5555-555555555555", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, true, false, false, null, "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==", null, null, null, false, "b1a1d25f-8a7e-4e9a-bc55-8dca5bfa1234", false, null, null, "admin@admin.com", "admin" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), 0, "66666666-6666-6666-6666-666666666666", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, true, false, false, null, "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==", null, null, "+905551112233", true, "0fa3f1d8-e77f-4aa9-9f12-6f8c7f90a002", false, null, null, "editor@blogapp.dev", "editor_lara" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), 0, "77777777-7777-7777-7777-777777777777", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, true, false, false, null, "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==", null, null, null, false, "7c1dbdbb-3d91-45a2-8578-5392cda53875", false, null, null, "moderator@blogapp.dev", "moderator_selim" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), 0, "88888888-8888-8888-8888-888888888888", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc), null, true, false, false, null, "AQAAAAIAAYagAAAAEP8xlsKNntQQ1SivmqfdllQWKX/655QCNjrVsPYL/Oz4cUgmI8aV55GO0BN9SDNltA==", null, null, "+905559998877", true, "e8de6375-bbb3-4ac6-a5dd-8530b7072d86", false, null, null, "author@blogapp.dev", "author_melike" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Body", "CategoryId", "CreatedById", "CreatedDate", "DeletedDate", "IsDeleted", "IsPublished", "Summary", "Thumbnail", "Title", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "OpenTelemetry Collector, distributed tracing ve yapılandırılmış logging birleştiğinde minimal API'ler üretim ortamında şeffaf hâle geliyor.\r\nBu rehberde ActivitySource, Meter ve TraceId bağlamlarını nasıl kodladığımızı adım adım ele alıyoruz.\r\nAyrıca Aspire Dashboard ile gecikme ve hata oranlarını anlık izlemenin püf noktalarını paylaşıyoruz.", new Guid("10000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 9, 28, 8, 30, 0, 0, DateTimeKind.Utc), null, false, true, "OpenTelemetry ve Aspire ile ASP.NET Core minimal API projelerinde uçtan uca gözlemlenebilirlik altyapısını kurma rehberi.", "https://miro.medium.com/v2/resize:fit:1286/format:webp/1*chhJLW0ApPDHqmVPRBBUtQ.png", ".NET 9 Minimal API'lerinde Observability Entegrasyonu", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "SaaS uygulamalarında sorgu optimizasyonu tenant bazında indeksleme ile başlıyor.\r\nModel seeding, global query filter'lar ve concurrency token'ları üzerinden performans analizleri paylaşıyoruz.\r\nAyrıca Npgsql provider'ı ile partitioned table senaryolarını örneklendiriyoruz.", new Guid("10000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 2, 9, 15, 0, 0, DateTimeKind.Utc), null, false, true, "Tenant bazlı filtreleme, connection pooling ve caching stratejileriyle çoklu tenant SaaS uygulamalarında EF Core'u hızlandırma.", "https://miro.medium.com/0*V56TEDMUsms9XLBY.jpg", "EF Core 9 ile Çoklu Tenant Mimarisinde Performans İpuçları", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "GitOps, manifest kaynağını tek gerçeğin kaynağına dönüştürerek roll-forward ve roll-back süreçlerini sadeleştiriyor.\r\nFluxCD ile progressive delivery, ArgoCD ile health check politika tanımlarını örnek YAML dosyalarıyla açıklıyoruz.\r\nPipeline gözlemlenebilirliği için Prometheus ve Grafana entegrasyonlarını da ekliyoruz.", new Guid("10000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 5, 10, 45, 0, 0, DateTimeKind.Utc), null, false, true, "FluxCD ve ArgoCD karşılaştırmasıyla GitOps pipeline'larını teknoloji blogu projelerine uyarlama.", "https://miro.medium.com/v2/resize:fit:4800/format:webp/1*9P6wnky3C9xMwaBAElALLQ.png", "GitOps ile Kubernetes Üzerinde Sürekli Teslimat", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Mesajlaşma altyapısı seçiminde gereksinimleri segmentlere ayırmak kritik.\r\nRabbitMQ routing esnekliği sağlar; Kafka ise sıralı event log ile akış analitiğine güç katar.\r\nMakale boyunca tüketici grupları, dead-letter stratejileri ve metrik takip yöntemlerini detaylandırıyoruz.", new Guid("10000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 12, 15, 0, 0, 0, DateTimeKind.Utc), null, false, false, "Etkinlik yönelimli servisler için mesajlaşma altyapısı seçerken throughput, gecikme ve gözlemlenebilirlik kriterlerinin karşılaştırılması.", "https://miro.medium.com/1*BGZy6Puq8X9AQHkJzB7BLA.jpeg", "Event-Driven Mimarilerde RabbitMQ mu Kafka mı?", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000005"), "Tracing zincirleri, metrik korelasyonları ve yapılandırılmış log'lar aynı veri modelinde buluştuğunda kök neden analizi hızlanıyor.\r\nBu makalede collector konfigürasyonlarını, OTLP protokolünü ve Prometheus remote write senaryolarını harmanlıyoruz.\r\nEk olarak, kullanıcı segmenti bazlı alert kurallarına dair pratik şablonlar sunuyoruz.", new Guid("10000000-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 15, 11, 20, 0, 0, DateTimeKind.Utc), null, false, true, "Teknoloji blogu altyapısı için tracing, metrics ve logging verilerini aynı veri gölünde birleştirme stratejileri.", "https://miro.medium.com/v2/resize:fit:1400/format:webp/1*zHc9d823Uol9SSj8s_uBug.png", "OpenTelemetry ile Katmanlı Gözlemlenebilirlik", null, null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "GrantedAt" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000005"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000007"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000008"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000009"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000010"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000011"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000012"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000022"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000028"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000030"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000031"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000032"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000033"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000034"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000035"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000036"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000024"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000026"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000028"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000013"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000014"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000015"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000016"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000017"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000018"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000019"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000020"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000021"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000023"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000025"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000027"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("30000000-0000-0000-0000-000000000029"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedDate" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2025, 10, 23, 7, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "CommentOwnerMail", "Content", "CreatedById", "CreatedDate", "DeletedDate", "IsDeleted", "IsPublished", "ParentId", "PostId", "UpdatedById", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), "techreader@blogapp.dev", "Minimal API'lerde TraceId propagasyonu için örnek kod parçalarını uyguladım, Aspire Dashboard harika çalıştı!", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 20, 9, 0, 0, 0, DateTimeKind.Utc), null, false, true, null, new Guid("40000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "community@blogapp.dev", "Kafka bölümündeki partition senaryosunu canlı görmek isterim, yayınlandığında haber verir misiniz?", new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2025, 10, 20, 9, 40, 0, 0, DateTimeKind.Utc), null, false, false, null, new Guid("40000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "editor@blogapp.dev", "Trace örneklerinin yanına log seviyeleri ekleyince sorun tespiti çok hızlandı, teşekkürler!", new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2025, 10, 20, 9, 12, 0, 0, DateTimeKind.Utc), null, false, true, new Guid("60000000-0000-0000-0000-000000000001"), new Guid("40000000-0000-0000-0000-000000000001"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Entity",
                table: "ActivityLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_Timestamp",
                table: "ActivityLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedDate",
                table: "Comments",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsPublished",
                table: "Comments",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_IsPublished",
                table: "Comments",
                columns: new[] { "PostId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_CreatedAt",
                table: "OutboxMessages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt",
                table: "OutboxMessages",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAt_NextRetryAt",
                table: "OutboxMessages",
                columns: new[] { "ProcessedAt", "NextRetryAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Type",
                table: "Permissions",
                columns: new[] { "Module", "Type" });

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
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

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
                name: "IX_RefreshSessions_Jti",
                table: "RefreshSessions",
                column: "Jti");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_TokenHash",
                table: "RefreshSessions",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_UserId_DeviceId",
                table: "RefreshSessions",
                columns: new[] { "UserId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUserName",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "BookshelfItems");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "RefreshSessions");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
