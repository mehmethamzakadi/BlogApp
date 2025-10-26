using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IUserRepository : IAsyncRepository<User>, IRepository<User>
{
    User? FindById(Guid id);
    Task<User?> FindByIdAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUserNameAsync(string userName);
    Task<IResult> CreateAsync(User user, string password);
    Task<IResult> AddToRoleAsync(User user, string roleName);
    Task<IResult> AddToRolesAsync(User user, params string[] roles);
    Task<IResult> RemoveFromRolesAsync(User user, params string[] roles);
    new Task<IResult> UpdateAsync(User user);
    Task<IResult> DeleteUserAsync(User user);
    Task<IResult> UpdatePasswordAsync(Guid userId, string resetToken, string newPassword);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<List<string>> GetRolesAsync(User user);
    Task<List<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Paginate<User>> GetUsersAsync(int index, int size, CancellationToken cancellationToken);
}
