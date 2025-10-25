using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using System.Collections.Generic;

namespace BlogApp.Domain.Repositories;

/// <summary>
/// Permission entity için repository interface
/// </summary>
public interface IPermissionRepository : IAsyncRepository<Permission>, IRepository<Permission>
{
    /// <summary>
    /// Belirli rol ID'lerine ait tüm permission'ları getirir
    /// </summary>
    Task<List<Permission>> GetPermissionsByRoleIdsAsync(List<int> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// İsme göre permission getirir
    /// </summary>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Role ait permission'ları getirir
    /// </summary>
    Task<List<AppRolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir role permission atar (tüm eski permission'ları replace eder)
    /// </summary>
    Task AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default);
}
