using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Extentions;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Persistence.Services;

public sealed class UserService(UserManager<AppUser> userManager) : IUserService
{
    public async Task AddToRoleAsync(AppUser user, string userRoles)
    {
        await userManager.AddToRoleAsync(user, userRoles);
    }

    public async Task<IdentityResult> CreateAsync(AppUser user, string password)
    {
        IdentityResult result = await userManager.CreateAsync(user, password);
        return result;
    }

    public async Task<IdentityResult> DeleteAsync(AppUser user)
    {
        return await userManager.DeleteAsync(user);
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public AppUser? FindById(int id)
    {
        var response = userManager.Users.Where(x => x.Id == id).FirstOrDefault();
        return response;
    }

    public async Task<Paginate<AppUser>> GetUsers(int index, int size, CancellationToken cancellationToken)
    {
        return await userManager.Users.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<IdentityResult> UpdateAsync(AppUser user)
    {
        var response = await userManager.UpdateAsync(user);
        return response;
    }

    public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
    {
        AppUser? user = await userManager.FindByIdAsync(userId);

        if (user != null)
        {
            resetToken = resetToken.UrlDecode();
            IdentityResult result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (result.Succeeded)
                await userManager.UpdateSecurityStampAsync(user);
            else
                throw new PasswordChangeFailedException();

        }
    }
}
