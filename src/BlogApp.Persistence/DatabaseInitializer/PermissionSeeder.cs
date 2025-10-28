using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Persistence.DatabaseInitializer;

/// <summary>
/// Permission'ları otomatik olarak database'e seed eden service
/// Uygulama başlangıcında çalışır ve eksik permission'ları ekler
/// </summary>
public class PermissionSeeder
{
    private readonly BlogAppDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<PermissionSeeder> _logger;

    public PermissionSeeder(
        BlogAppDbContext context,
        IRoleRepository roleRepository,
        ILogger<PermissionSeeder> logger)
    {
        _context = context;
        _roleRepository = roleRepository;
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

        // Önce tüm mevcut permission'ları al (IsDeleted kontrolü ile)
        var existingPermissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        var existingPermissionNames = existingPermissions.Select(p => p.Name).ToHashSet();

        var missingPermissions = allPermissionNames
                 .Except(existingPermissionNames)
                 .ToList();

        if (!missingPermissions.Any())
        {
            _logger.LogInformation("All permissions already exist in database");
            return;
        }

        _logger.LogInformation($"Adding {missingPermissions.Count} missing permissions");

        foreach (var permissionName in missingPermissions)
        {
            // Tekrar kontrol et - race condition için
            var exists = existingPermissions.Any(p => p.Name == permissionName);
            if (exists)
            {
                _logger.LogInformation($"Permission {permissionName} already exists, skipping");
                continue;
            }

            var parts = permissionName.Split('.');
            var module = parts[0];
            var type = parts.Length > 1 ? parts[1] : "Custom";

            var permission = new Permission
            {
                Name = permissionName,
                NormalizedName = permissionName.ToUpperInvariant(),
                Module = module,
                Type = type,
                Description = GetPermissionDescription(permissionName),
                CreatedDate = DateTime.UtcNow,
                CreatedById = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                IsDeleted = false
            };

            _context.Permissions.Add(permission);
        }

        // SaveChanges'ı try-catch içinde yap
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully added {missingPermissions.Count} permissions");
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
        {
            _logger.LogWarning("Duplicate key detected during permission seeding. Some permissions may already exist.");
            // Duplicate key hatası gelirse, context'i temizle ve devam et
            _context.ChangeTracker.Clear();
        }
    }

    private async Task AssignAllPermissionsToAdminAsync()
    {
        var adminRole = await _roleRepository.Query()
            .FirstOrDefaultAsync(r => r.NormalizedName == UserRoles.Admin.ToUpper());

        if (adminRole == null)
        {
            _logger.LogWarning("Admin role not found");
            return;
        }

        var allPermissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        var existingRolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        // Sadece eksik olan permission'ları ekle, var olanları dokunma
        var permissionsToAdd = allPermissions
            .Where(p => !existingRolePermissions.Contains(p.Id))
            .Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow
            })
            .ToList();

        if (permissionsToAdd.Any())
        {
            _context.RolePermissions.AddRange(permissionsToAdd);
            _logger.LogInformation($"Assigned {permissionsToAdd.Count} new permissions to Admin role");
        }
        else
        {
            _logger.LogInformation("Admin role already has all permissions");
        }
    }

    private async Task AssignBasicPermissionsToUserAsync()
    {
        var userRole = await _roleRepository.Query()
            .FirstOrDefaultAsync(r => r.NormalizedName == UserRoles.User.ToUpper());

        if (userRole == null)
        {
            _logger.LogWarning("User role not found");
            return;
        }

        // User rolünün zaten permission'ları varsa, manuel değişiklikleri korumak için seed işlemini atla
        var hasExistingPermissions = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == userRole.Id);

        if (hasExistingPermissions)
        {
            _logger.LogInformation("User role already has permissions assigned. Skipping default permission assignment to preserve manual changes.");
            return;
        }

        // İlk kurulumda User rolüne temel permission'ları ata
        var userPermissionNames = Permissions.GetUserPermissions();
        var userPermissions = await _context.Permissions
            .Where(p => userPermissionNames.Contains(p.Name) && !p.IsDeleted)
            .ToListAsync();

        var permissionsToAdd = userPermissions
            .Select(p => new RolePermission
            {
                RoleId = userRole.Id,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow
            })
            .ToList();

        if (permissionsToAdd.Any())
        {
            _context.RolePermissions.AddRange(permissionsToAdd);
            _logger.LogInformation($"Assigned {permissionsToAdd.Count} default permissions to User role (first-time setup)");
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
            Permissions.BookshelfCreate => "Yeni kitap kaydı oluşturma yetkisi",
            Permissions.BookshelfRead => "Kitap kayıtlarını görüntüleme yetkisi",
            Permissions.BookshelfUpdate => "Kitap kayıtlarını güncelleme yetkisi",
            Permissions.BookshelfDelete => "Kitap kaydı silme yetkisi",
            Permissions.BookshelfViewAll => "Tüm kitap kayıtlarını görüntüleme yetkisi",
            Permissions.ActivityLogsView => "Aktivite loglarını görüntüleme yetkisi",
            _ => $"{permissionName} permission"
        };
    }
}
