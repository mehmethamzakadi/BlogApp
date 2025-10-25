using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BlogApp.Persistence.Repositories;

public class PermissionRepository : EfRepositoryBase<Permission, BlogAppDbContext>, IPermissionRepository
{
    public PermissionRepository(BlogAppDbContext context) : base(context)
    {
    }

    public async Task<List<Permission>> GetPermissionsByRoleIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default)
    {
        return await Context.AppRolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Context.Permissions
            .FirstOrDefaultAsync(p => p.Name == name && !p.IsDeleted, cancellationToken);
    }

    public async Task<List<AppRolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await Context.AppRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        // Var olan permission'ları sil
        var existingPermissions = await Context.AppRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        Context.AppRolePermissions.RemoveRange(existingPermissions);

        // Yeni permission'ları ekle
        if (permissionIds.Any())
        {
            var newPermissions = permissionIds.Select(permissionId => new AppRolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedAt = DateTime.UtcNow
            }).ToList();

            await Context.AppRolePermissions.AddRangeAsync(newPermissions, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }
}
