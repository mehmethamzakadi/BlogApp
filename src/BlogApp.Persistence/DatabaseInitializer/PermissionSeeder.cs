using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer;

/// <summary>
/// Permission'ları otomatik olarak database'e seed eden service
/// Uygulama başlangıcında çalışır ve eksik permission'ları ekler
/// </summary>
public class PermissionSeeder
{
    private readonly BlogAppDbContext _context;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<PermissionSeeder> _logger;

    public PermissionSeeder(
        BlogAppDbContext context,
        RoleManager<AppRole> roleManager,
        ILogger<PermissionSeeder> logger)
    {
        _context = context;
        _roleManager = roleManager;
        _logger = logger;
    }

    /// <summary>
    /// Permission'ları seed eder ve default rol izinlerini atar
    /// </summary>
    public async Task SeedPermissionsAsync()
    {
        try
        {
            _logger.LogInformation("Starting permission seeding process");

            // 1. Permission'ları kontrol et ve oluştur
            await EnsurePermissionsExistAsync();
            await _context.SaveChangesAsync(); // ✅ İlk önce permission'ları kaydet
            _logger.LogInformation("Permissions saved to database");

            // 2. Admin rolüne tüm permission'ları ata
            await AssignAllPermissionsToAdminAsync();
            await _context.SaveChangesAsync(); // ✅ Admin permission'larını kaydet
            _logger.LogInformation("Admin permissions assigned");

            // 3. User rolüne temel permission'ları ata
            await AssignBasicPermissionsToUserAsync();
            await _context.SaveChangesAsync(); // ✅ User permission'larını kaydet
            _logger.LogInformation("User permissions assigned");

            _logger.LogInformation("Permission seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding permissions");
            throw;
        }
    }

    private async Task EnsurePermissionsExistAsync()
    {
        var allPermissionNames = Permissions.GetAllPermissions();
        var existingPermissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .Select(p => p.Name)
            .ToListAsync();

        var missingPermissions = allPermissionNames
            .Except(existingPermissions)
            .ToList();

        if (!missingPermissions.Any())
        {
            _logger.LogInformation("All permissions already exist in database");
            return;
        }

        _logger.LogInformation($"Adding {missingPermissions.Count} missing permissions");

        foreach (var permissionName in missingPermissions)
        {
            var parts = permissionName.Split('.');
            var module = parts[0];
            var type = parts.Length > 1 ? parts[1] : "Custom";

            var permission = new Permission
            {
                Name = permissionName,
                Module = module,
                Type = type,
                Description = GetPermissionDescription(permissionName),
                CreatedDate = DateTime.UtcNow,
                CreatedById = 1,
                IsDeleted = false
            };

            _context.Permissions.Add(permission);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Successfully added {missingPermissions.Count} permissions");
    }

    private async Task AssignAllPermissionsToAdminAsync()
    {
        var adminRole = await _roleManager.FindByNameAsync(UserRoles.Admin);
        if (adminRole == null)
        {
            _logger.LogWarning("Admin role not found");
            return;
        }

        var allPermissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        var existingRolePermissions = await _context.AppRolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var permissionsToAdd = allPermissions
            .Where(p => !existingRolePermissions.Contains(p.Id))
            .Select(p => new AppRolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow
            })
            .ToList();

        if (permissionsToAdd.Any())
        {
            _context.AppRolePermissions.AddRange(permissionsToAdd);
            _logger.LogInformation($"Assigned {permissionsToAdd.Count} permissions to Admin role");
        }
    }

    private async Task AssignBasicPermissionsToUserAsync()
    {
        var userRole = await _roleManager.FindByNameAsync(UserRoles.User);
        if (userRole == null)
        {
            _logger.LogWarning("User role not found");
            return;
        }

        var userPermissionNames = Permissions.GetUserPermissions();
        var userPermissions = await _context.Permissions
            .Where(p => userPermissionNames.Contains(p.Name) && !p.IsDeleted)
            .ToListAsync();

        var existingRolePermissions = await _context.AppRolePermissions
            .Where(rp => rp.RoleId == userRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        var permissionsToAdd = userPermissions
            .Where(p => !existingRolePermissions.Contains(p.Id))
            .Select(p => new AppRolePermission
            {
                RoleId = userRole.Id,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow
            })
            .ToList();

        if (permissionsToAdd.Any())
        {
            _context.AppRolePermissions.AddRange(permissionsToAdd);
            _logger.LogInformation($"Assigned {permissionsToAdd.Count} permissions to User role");
        }
    }

    private string GetPermissionDescription(string permissionName)
    {
        return permissionName switch
        {
            Permissions.DashboardView => "Admin paneli dashboard'una erişim yetkisi",
            Permissions.UsersCreate => "Yeni kullanıcı oluşturma yetkisi",
            Permissions.UsersRead => "Kullanıcı bilgilerini görüntüleme yetkisi",
            Permissions.UsersUpdate => "Kullanıcı bilgilerini güncelleme yetkisi",
            Permissions.UsersDelete => "Kullanıcı silme yetkisi",
            Permissions.UsersViewAll => "Tüm kullanıcıları görüntüleme yetkisi",
            Permissions.RolesCreate => "Yeni rol oluşturma yetkisi",
            Permissions.RolesRead => "Rol bilgilerini görüntüleme yetkisi",
            Permissions.RolesUpdate => "Rol bilgilerini güncelleme yetkisi",
            Permissions.RolesDelete => "Rol silme yetkisi",
            Permissions.RolesViewAll => "Tüm rolleri görüntüleme yetkisi",
            Permissions.RolesAssignPermissions => "Role yetki atama yetkisi",
            Permissions.PostsCreate => "Yeni post oluşturma yetkisi",
            Permissions.PostsRead => "Post görüntüleme yetkisi",
            Permissions.PostsUpdate => "Post güncelleme yetkisi",
            Permissions.PostsDelete => "Post silme yetkisi",
            Permissions.PostsViewAll => "Tüm postları görüntüleme yetkisi",
            Permissions.PostsPublish => "Post yayınlama yetkisi",
            Permissions.CategoriesCreate => "Yeni kategori oluşturma yetkisi",
            Permissions.CategoriesRead => "Kategori görüntüleme yetkisi",
            Permissions.CategoriesUpdate => "Kategori güncelleme yetkisi",
            Permissions.CategoriesDelete => "Kategori silme yetkisi",
            Permissions.CategoriesViewAll => "Tüm kategorileri görüntüleme yetkisi",
            Permissions.CommentsCreate => "Yeni yorum oluşturma yetkisi",
            Permissions.CommentsRead => "Yorum görüntüleme yetkisi",
            Permissions.CommentsUpdate => "Yorum güncelleme yetkisi",
            Permissions.CommentsDelete => "Yorum silme yetkisi",
            Permissions.CommentsViewAll => "Tüm yorumları görüntüleme yetkisi",
            Permissions.CommentsModerate => "Yorum moderasyon yetkisi",
            _ => $"{permissionName} permission"
        };
    }
}
