using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Abstractions.Identity;

public interface IRoleService
{
    Task<Paginate<AppRole>> GetRoles(int index, int size, CancellationToken cancellationToken);
    AppRole? GetRoleById(int id);
    Task<IdentityResult> CreateRole(AppRole role);
    Task<IdentityResult> DeleteRole(AppRole role);
    Task<IdentityResult> UpdateRole(AppRole role);
    bool AnyRole(string name);
}
