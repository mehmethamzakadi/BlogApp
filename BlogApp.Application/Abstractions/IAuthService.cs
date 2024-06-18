using BlogApp.Application.Features.AppUsers.Commands.Login;
using BlogApp.Domain.Common.Results;

namespace BlogApp.Application.Abstractions;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task PasswordResetAsync(string email);
    Task<Result<bool>> PasswordVerify(string resetToken, string userId);
}
