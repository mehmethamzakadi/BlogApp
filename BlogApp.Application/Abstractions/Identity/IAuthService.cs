using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common.Results;

namespace BlogApp.Application.Abstractions.Identity;

public interface IAuthService
{
    Task<IDataResult<LoginResponse>> LoginAsync(string email, string password);
    Task PasswordResetAsync(string email);
    Task<IDataResult<bool>> PasswordVerify(string resetToken, string userId);
}
