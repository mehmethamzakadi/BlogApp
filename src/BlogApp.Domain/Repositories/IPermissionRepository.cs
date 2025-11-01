using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

/// <summary>
/// Permission entity için repository interface
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    /// Belirli rol ID'lerine ait tüm permission'ları getirir
    /// </summary>
    Task<List<Permission>> GetPermissionsByRoleIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// İsme göre permission getirir
    /// </summary>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Role ait permission'ları getirir
    /// </summary>
    Task<List<RolePermission>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir role permission atar (tüm eski permission'ları replace eder)
    /// </summary>
    Task AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get permissions by IDs
    /// </summary>
    Task<List<Permission>> GetByIdsAsync(List<Guid> permissionIds, CancellationToken cancellationToken = default);
}
