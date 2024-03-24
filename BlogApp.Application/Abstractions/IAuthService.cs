using BlogApp.Application.Features.Authorizations.Commands.UserLogin;
using BlogApp.Domain.Common.Results;

namespace BlogApp.Application.Abstractions;

public interface IAuthService
{
    Task<IDataResult<TokenResponse>> LoginAsync(string email, string password);
    Task PasswordResetAsync(string email);
    Task<IDataResult<bool>> PasswordVerify(string resetToken, string userId);
}
