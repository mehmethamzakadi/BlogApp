using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Extentions;
using BlogApp.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Services.Identity;

public sealed class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    ITokenService tokenService,
    IMailService mailService,
    BlogAppDbContext dbContext) : IAuthService
{
    public async Task<IDataResult<LoginResponse>> LoginAsync(string email, string password)
    {
        AppUser? user = await userManager.FindByEmailAsync(email) ?? throw new AuthenticationErrorException();

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (signInResult.IsLockedOut)
        {
            throw new AuthenticationErrorException("Hesabınız çok sayıda hatalı giriş nedeniyle kilitlendi.");
        }

        if (signInResult.RequiresTwoFactor)
        {
            throw new AuthenticationErrorException("İki faktörlü doğrulama gereklidir.");
        }

        if (!signInResult.Succeeded)
        {
            throw new AuthenticationErrorException();
        }

        var authClaims = await tokenService.GetAuthClaims(user);
        var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);
        await signInManager.SignInWithClaimsAsync(user, false, authClaims);

        await userManager.RemoveAuthenticationTokenAsync(user, "BlogApp", "RefreshToken");
        await userManager.SetAuthenticationTokenAsync(user, "BlogApp", "RefreshToken", tokenResponse.RefreshToken);

        return new SuccessDataResult<LoginResponse>(tokenResponse, "Giriş Başarılı");
    }

    public async Task<IDataResult<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        // ✅ OPTIMIZED: Query the token table directly instead of loading all users
        // This prevents N+1 query and scales better with large user bases
        var userToken = await dbContext.AppUserTokens
            .FirstOrDefaultAsync(t =>
                t.LoginProvider == "BlogApp" &&
                t.Name == "RefreshToken" &&
                t.Value == refreshToken);

        if (userToken == null)
        {
            throw new AuthenticationErrorException("Geçersiz refresh token.");
        }

        var user = await userManager.FindByIdAsync(userToken.UserId.ToString());

        if (user == null)
        {
            throw new AuthenticationErrorException("Geçersiz refresh token.");
        }

        // Yeni token oluştur
        var authClaims = await tokenService.GetAuthClaims(user);
        var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);

        // Eski refresh token'ı kaldır ve yenisini kaydet
        await userManager.RemoveAuthenticationTokenAsync(user, "BlogApp", "RefreshToken");
        await userManager.SetAuthenticationTokenAsync(user, "BlogApp", "RefreshToken", tokenResponse.RefreshToken);

        return new SuccessDataResult<LoginResponse>(tokenResponse, "Token yenilendi");
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

    public async Task<IDataResult<bool>> PasswordVerify(string resetToken, string userId)
    {
        AppUser? user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            resetToken = resetToken.UrlDecode();
            var result = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            return new SuccessDataResult<bool>(result);
        }
        return new SuccessDataResult<bool>(false);
    }
}
