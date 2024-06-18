using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.AppUsers.Commands.Login;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Extentions;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Persistence.Services;

public sealed class AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMailService mailService) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        AppUser? user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            bool checkPassword = await userManager.CheckPasswordAsync(user, password);
            if (!checkPassword)
                return Result<LoginResponse>.FailureResult("Email veya Şifre Hatalı!");

            var authClaims = await tokenService.GetAuthClaims(user);
            var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);
            await signInManager.SignInWithClaimsAsync(user, false, authClaims);

            await userManager.RemoveAuthenticationTokenAsync(user, "BlogApp", "RefreshToken");
            await userManager.SetAuthenticationTokenAsync(user, "BlogApp", "RefreshToken", tokenResponse.RefreshToken);

            return Result<LoginResponse>.SuccessResult(tokenResponse, "Giriş Başarılı");
        }
        return Result<LoginResponse>.FailureResult("Email veya Şifre Hatalı!");
    }

    public async Task PasswordResetAsync(string email)
    {
        AppUser? user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            resetToken = resetToken.UrlEncode();
            await mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
        }
    }

    public async Task<Result<bool>> PasswordVerify(string resetToken, string userId)
    {
        AppUser? user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            resetToken = resetToken.UrlDecode();
            var result = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            return Result<bool>.SuccessResult(result, "Şifre Doğrulandı.");
        }
        return Result<bool>.FailureResult("Kullanıcı Bulunamadı.");
    }
}
