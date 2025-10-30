using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Paginate<Role>> GetRoles(int index, int size, CancellationToken cancellationToken);
    Role? GetRoleById(Guid id);
    Task<Role?> FindByNameAsync(string roleName);
    Task<IResult> CreateRole(Role role);
    Task<IResult> DeleteRole(Role role);
    Task<IResult> UpdateRole(Role role);
    bool AnyRole(string name);
}
