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

    public async Task<(int id, string name)> GetRoleById(int id)
    {
        string role = await roleManager.GetRoleIdAsync(new() { Id = id });
        return (id, role);
    }

    public async Task<bool> CreateRole(string name)
    {
        IdentityResult result = await roleManager.CreateAsync(new() { Name = name });
        return result.Succeeded;
    }

    public async Task<bool> DeleteRole(string name)
    {
        IdentityResult result = await roleManager.DeleteAsync(new() { Name = name });
        return result.Succeeded;
    }

    public async Task<bool> UpdateRole(int id, string name)
    {
        IdentityResult result = await roleManager.UpdateAsync(new() { Id = id, Name = name });
        return result.Succeeded;
    }
}
