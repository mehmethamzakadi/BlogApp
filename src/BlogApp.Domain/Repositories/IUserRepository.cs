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
    Task<List<UserRole>> GetAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SoftDeleteUserRolesAsync(List<UserRole> userRoles, CancellationToken cancellationToken = default);
    Task RestoreUserRoleAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task AddUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
