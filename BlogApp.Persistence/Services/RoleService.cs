using BlogApp.Application.Abstractions;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace BlogApp.Persistence.Services;

public sealed class RoleService(RoleManager<AppRole> roleManager) : IRoleService
{
    public IDictionary<int, string?>? GetAllRoles()
    {
        var roles = roleManager.Roles.ToDictionary(role => role.Id, role => role.Name);
        return roles;
    }

    public async Task<string> GetRoleById(AppRole role)
    {
        return await roleManager.GetRoleIdAsync(role);
    }

    public async Task<bool> CreateRole(AppRole role)
    {
        IdentityResult result = await roleManager.CreateAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> DeleteRole(AppRole role)
    {
        IdentityResult result = await roleManager.DeleteAsync(role);
        return result.Succeeded;
    }

    public async Task<bool> UpdateRole(AppRole role)
    {
        IdentityResult result = await roleManager.UpdateAsync(role);
        return result.Succeeded;
    }
}
