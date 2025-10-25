using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IUserRepository : IAsyncRepository<User>, IRepository<User>
{
    User? FindById(int id);
    Task<User?> FindByIdAsync(int id);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUserNameAsync(string userName);
    Task<IResult> CreateAsync(User user, string password);
    Task<IResult> AddToRoleAsync(User user, string roleName);
    Task<IResult> AddToRolesAsync(User user, params string[] roles);
    Task<IResult> RemoveFromRolesAsync(User user, params string[] roles);
    new Task<IResult> UpdateAsync(User user);
    Task<IResult> DeleteUserAsync(User user);
    Task<IResult> UpdatePasswordAsync(int userId, string resetToken, string newPassword);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<List<string>> GetRolesAsync(User user);
    Task<Paginate<User>> GetUsersAsync(int index, int size, CancellationToken cancellationToken);
}
