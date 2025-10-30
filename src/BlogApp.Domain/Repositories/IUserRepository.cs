using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<Paginate<User>> GetUsersAsync(int index, int size, CancellationToken cancellationToken);
    Task<User?> FindByIdAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUserNameAsync(string userName);
    Task<List<string>> GetRolesAsync(User user);
    Task<List<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);
}
