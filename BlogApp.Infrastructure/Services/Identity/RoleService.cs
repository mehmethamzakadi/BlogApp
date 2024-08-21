using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
namespace BlogApp.Infrastructure.Services.Identity;

public sealed class RoleService(RoleManager<AppRole> roleManager) : IRoleService
{
    public async Task<Paginate<AppRole>> GetRoles(int index, int size, CancellationToken cancellationToken)
    {
        return await roleManager.Roles.ToPaginateAsync(index, size, cancellationToken);
    }

    public AppRole? GetRoleById(int id)
    {
        var result = roleManager.Roles
            .Where(x => x.Id == id)
            .Select(x => new AppRole { Id = x.Id, Name = x.Name })
            .FirstOrDefault();
        return result;
    }

    public async Task<IdentityResult> CreateRole(AppRole role)
    {
        IdentityResult result = await roleManager.CreateAsync(role);
        return result;
    }

    public async Task<IdentityResult> DeleteRole(AppRole role)
    {
        IdentityResult result = await roleManager.DeleteAsync(role);
        return result;
    }

    public async Task<IdentityResult> UpdateRole(AppRole role)
    {
        IdentityResult result = await roleManager.UpdateAsync(role);
        return result;
    }

    public bool AnyRole(string name)
    {
        var result = roleManager.Roles
            .Any(x => x.Name == name);

        return result;
    }
}
