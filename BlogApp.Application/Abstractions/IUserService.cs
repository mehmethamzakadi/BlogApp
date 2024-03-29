using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Abstractions;

public interface IUserService
{
    Task<Paginate<AppUser>> GetUsers(int index, int size, CancellationToken cancellationToken);
    AppUser? FindById(int id);
    Task<AppUser?> FindByEmailAsync(string email);
    Task AddToRoleAsync(AppUser user, string userRoles);
    Task<IdentityResult> CreateAsync(AppUser user, string password);
    Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);
    Task<IdentityResult> DeleteAsync(AppUser user);
    Task<IdentityResult> UpdateAsync(AppUser user);
}
