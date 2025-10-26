using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Seeds;

public class PermissionSeed : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        var seedDate = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);
        var systemUserId = SystemUsers.SystemUserId;

        var permissionRecords = Permissions.GetAllPermissions()
            .Select((permissionName, index) =>
            {
                var parts = permissionName.Split('.', 2);
                var module = parts[0];
                var type = parts.Length > 1 ? parts[1] : "Custom";

                return new Permission
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-000000000{index + 1:D3}"),
                    Name = permissionName,
                    NormalizedName = permissionName.ToUpperInvariant(),
                    Module = module,
                    Type = type,
                    Description = GetPermissionDescription(permissionName),
                    CreatedById = systemUserId,
                    CreatedDate = seedDate,
                    IsDeleted = false
                };
            })
            .ToArray();

        builder.HasData(permissionRecords);
    }

    private static string GetPermissionDescription(string permissionName)
    {
        return permissionName switch
        {
            Permissions.DashboardView => "Yönetim paneli dashboard ekranına erişim.",

            // User management
            Permissions.UsersCreate => "Yeni kullanıcı oluşturma yetkisi.",
            Permissions.UsersRead => "Kullanıcı bilgilerini görüntüleme yetkisi.",
            Permissions.UsersUpdate => "Kullanıcı bilgilerini güncelleme yetkisi.",
            Permissions.UsersDelete => "Kullanıcı silme yetkisi.",
            Permissions.UsersViewAll => "Tüm kullanıcıları görüntüleme yetkisi.",

            // Role management
            Permissions.RolesCreate => "Yeni rol oluşturma yetkisi.",
            Permissions.RolesRead => "Rolleri görüntüleme yetkisi.",
            Permissions.RolesUpdate => "Rolleri güncelleme yetkisi.",
            Permissions.RolesDelete => "Rol silme yetkisi.",
            Permissions.RolesViewAll => "Tüm rolleri görüntüleme yetkisi.",
            Permissions.RolesAssignPermissions => "Rollere yetki atama yetkisi.",

            // Post management
            Permissions.PostsCreate => "Yeni blog yazısı oluşturma yetkisi.",
            Permissions.PostsRead => "Blog yazılarını görüntüleme yetkisi.",
            Permissions.PostsUpdate => "Blog yazılarını güncelleme yetkisi.",
            Permissions.PostsDelete => "Blog yazısı silme yetkisi.",
            Permissions.PostsViewAll => "Tüm blog yazılarını görüntüleme yetkisi.",
            Permissions.PostsPublish => "Blog yazısı yayınlama yetkisi.",

            // Category management
            Permissions.CategoriesCreate => "Yeni kategori oluşturma yetkisi.",
            Permissions.CategoriesRead => "Kategorileri görüntüleme yetkisi.",
            Permissions.CategoriesUpdate => "Kategorileri güncelleme yetkisi.",
            Permissions.CategoriesDelete => "Kategori silme yetkisi.",
            Permissions.CategoriesViewAll => "Tüm kategorileri görüntüleme yetkisi.",

            // Comment management
            Permissions.CommentsCreate => "Yeni yorum oluşturma yetkisi.",
            Permissions.CommentsRead => "Yorumları görüntüleme yetkisi.",
            Permissions.CommentsUpdate => "Yorumları güncelleme yetkisi.",
            Permissions.CommentsDelete => "Yorum silme yetkisi.",
            Permissions.CommentsViewAll => "Tüm yorumları görüntüleme yetkisi.",
            Permissions.CommentsModerate => "Yorum moderasyonu yapma yetkisi.",

            // Bookshelf management
            Permissions.BookshelfCreate => "Yeni kitap kaydı oluşturma yetkisi.",
            Permissions.BookshelfRead => "Kitap kayıtlarını görüntüleme yetkisi.",
            Permissions.BookshelfUpdate => "Kitap kayıtlarını güncelleme yetkisi.",
            Permissions.BookshelfDelete => "Kitap kaydı silme yetkisi.",
            Permissions.BookshelfViewAll => "Tüm kitap kayıtlarını görüntüleme yetkisi.",

            // Activity logs
            Permissions.ActivityLogsView => "Aktivite loglarını görüntüleme yetkisi.",

            _ => $"{permissionName} yetkisi"
        };
    }
}
