using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public class PermissionRepository : EfRepositoryBase<Permission, BlogAppDbContext>, IPermissionRepository
{
    public PermissionRepository(BlogAppDbContext context) : base(context)
    {
    }

    public async Task<List<Permission>> GetPermissionsByRoleIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default)
    {
        return await Context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        // Case-insensitive arama için NormalizedName kullan
        var normalizedName = name.ToUpperInvariant();
        return await Context.Permissions
            .FirstOrDefaultAsync(p => p.NormalizedName == normalizedName && !p.IsDeleted, cancellationToken);
    }

    public async Task<List<RolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await Context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        // Var olan permission'ları sil
        var existingPermissions = await Context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        Context.RolePermissions.RemoveRange(existingPermissions);

        // Yeni permission'ları ekle
        if (permissionIds.Any())
        {
            var newPermissions = permissionIds.Select(permissionId => new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedAt = DateTime.UtcNow
            }).ToList();

            await Context.RolePermissions.AddRangeAsync(newPermissions, cancellationToken);
        }

        // ✅ FIXED: SaveChanges removed - UnitOfWork is responsible for transaction management
        // This ensures proper Outbox Pattern execution and maintains transaction integrity
    }
}
