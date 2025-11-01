using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer.Seeders;

/// <summary>
/// RolePermission ilişkilerini seed eder
/// Her role uygun permission'ları atar
/// </summary>
public class RolePermissionSeeder : BaseSeeder
{
    public RolePermissionSeeder(BlogAppDbContext context, ILogger<RolePermissionSeeder> logger) 
        : base(context, logger)
    {
    }

    public override int Order => 3; // Role ve Permission'dan sonra
    public override string Name => "RolePermission Seeder";

    protected override async Task SeedDataAsync(CancellationToken cancellationToken)
    {
        var grantedAt = new DateTime(2025, 10, 23, 7, 0, 0, DateTimeKind.Utc);

        // Permission ID mapping oluştur
        var permissions = await Context.Permissions
            .Where(p => !p.IsDeleted)
            .ToDictionaryAsync(p => p.Name, p => p.Id, cancellationToken);

        // Role ID'leri
        var adminRoleId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var userRoleId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var moderatorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var editorRoleId = Guid.Parse("20000000-0000-0000-0000-000000000004");

        var rolePermissions = new List<RolePermission>();

        // Admin - Tüm yetkiler
        AddPermissions(rolePermissions, adminRoleId, Permissions.GetAllPermissions(), permissions, grantedAt);

        // Editor - Post ve kategori yönetimi
        AddPermissions(rolePermissions, editorRoleId, new[]
        {
            Permissions.PostsCreate,
            Permissions.PostsRead,
            Permissions.PostsUpdate,
            Permissions.PostsDelete,
            Permissions.PostsViewAll,
            Permissions.PostsPublish,
            Permissions.CategoriesCreate,
            Permissions.CategoriesRead,
            Permissions.CategoriesUpdate,
            Permissions.CategoriesViewAll,
            Permissions.CommentsRead,
            Permissions.CommentsModerate,
            Permissions.CommentsDelete,
            Permissions.DashboardView
        }, permissions, grantedAt);

        // Moderator - İçerik moderasyonu
        AddPermissions(rolePermissions, moderatorRoleId, new[]
        {
            Permissions.CommentsRead,
            Permissions.CommentsViewAll,
            Permissions.CommentsModerate,
            Permissions.CommentsDelete,
            Permissions.PostsRead
        }, permissions, grantedAt);

        // User - Temel yetkiler
        AddPermissions(rolePermissions, userRoleId, new[]
        {
            Permissions.PostsCreate,
            Permissions.PostsRead,
            Permissions.PostsUpdate,
            Permissions.CategoriesRead,
            Permissions.CategoriesViewAll,
            Permissions.CommentsCreate,
            Permissions.CommentsRead,
            Permissions.CommentsUpdate
        }, permissions, grantedAt);

        // Mevcut role-permission ilişkilerini kontrol et
        var existingRelations = await Context.RolePermissions
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToHashSetAsync(cancellationToken);

        var newRolePermissions = rolePermissions
            .Where(rp => !existingRelations.Contains(new { rp.RoleId, rp.PermissionId }))
            .ToList();

        if (newRolePermissions.Any())
        {
            await Context.RolePermissions.AddRangeAsync(newRolePermissions, cancellationToken);
            Logger.LogInformation("Added {Count} new RolePermission relations", newRolePermissions.Count);
        }
        else
        {
            Logger.LogInformation("All RolePermission relations already exist, skipping");
        }
    }

    private void AddPermissions(
        List<RolePermission> rolePermissions,
        Guid roleId,
        IEnumerable<string> permissionNames,
        Dictionary<string, Guid> permissionMap,
        DateTime grantedAt)
    {
        foreach (var permissionName in permissionNames.Distinct())
        {
            if (permissionMap.TryGetValue(permissionName, out var permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    GrantedAt = grantedAt
                });
            }
            else
            {
                Logger.LogWarning("Permission '{PermissionName}' not found in database", permissionName);
            }
        }
    }
}
