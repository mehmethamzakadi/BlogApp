using BlogApp.Domain.Entities;

namespace BlogApp.Application.Abstractions;

public interface IRoleService
{
    IDictionary<int, string?>? GetAllRoles();
    Task<string> GetRoleById(AppRole role);
    Task<bool> CreateRole(AppRole role);
    Task<bool> DeleteRole(AppRole role);
    Task<bool> UpdateRole(AppRole role);
}
