using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Extentions;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Infrastructure.Services.Identity;

public sealed class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IMailService mailService,
    IPasswordHasher passwordHasher,
    BlogAppDbContext dbContext) : IAuthService
{
    public async Task<IDataResult<LoginResponse>> LoginAsync(string email, string password)
    {
        User? user = await userRepository.FindByEmailAsync(email)
            ?? throw new AuthenticationErrorException();

        // Check if account is locked
        if (user.IsLockedOut())
        {
            throw new AuthenticationErrorException("Hesabınız çok sayıda hatalı giriş nedeniyle kilitlendi.");
        }

        // Check if two factor is enabled
        if (user.TwoFactorEnabled)
        {
            throw new AuthenticationErrorException("İki faktörlü doğrulama gereklidir.");
        }

        // Verify password
        if (!await userRepository.CheckPasswordAsync(user, password))
        {
            // Increment failed access count
            user.AccessFailedCount++;

            // Lock account after 5 failed attempts
            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
            }

            await userRepository.UpdateAsync(user);
            throw new AuthenticationErrorException();
        }

        // Reset failed access count on successful login
        user.AccessFailedCount = 0;
        await userRepository.UpdateAsync(user);

        var authClaims = await tokenService.GetAuthClaims(user);
        var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);

        // Store refresh token
        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userRepository.UpdateAsync(user);

        return new SuccessDataResult<LoginResponse>(tokenResponse, "Giriş Başarılı");
    }

    public async Task<IDataResult<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await dbContext.Users
            .Where(u => !u.IsDeleted) // Soft delete kontrolü
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new AuthenticationErrorException("Geçersiz refresh token.");
        }

        // Check if account is locked
        if (user.IsLockedOut())
        {
            throw new AuthenticationErrorException("Hesabınız kilitlenmiş.");
        }

        // Generate new tokens
        var authClaims = await tokenService.GetAuthClaims(user);
        var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);

        // Update refresh token
        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userRepository.UpdateAsync(user);

        return new SuccessDataResult<LoginResponse>(tokenResponse, "Token yenilendi");
    }

    public async Task PasswordResetAsync(string email)
    {
        User? user = await userRepository.FindByEmailAsync(email);
        if (user != null)
        {
            string resetToken = passwordHasher.GeneratePasswordResetToken();
            user.PasswordResetToken = resetToken.UrlEncode();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await userRepository.UpdateAsync(user);
            await mailService.SendPasswordResetMailAsync(email, user.Id, resetToken.UrlEncode());
        }
    }

    public async Task<IDataResult<bool>> PasswordVerify(string resetToken, string userId)
    {
        if (!int.TryParse(userId, out int userIdInt))
        {
            return new SuccessDataResult<bool>(false);
        }

        User? user = await userRepository.FindByIdAsync(userIdInt);
        if (user != null && user.PasswordResetToken != null && user.PasswordResetTokenExpiry != null)
        {
            resetToken = resetToken.UrlDecode();
            var storedToken = user.PasswordResetToken.UrlDecode();

            if (storedToken == resetToken && user.PasswordResetTokenExpiry > DateTime.UtcNow)
            {
                return new SuccessDataResult<bool>(true);
            }
        }
        return new SuccessDataResult<bool>(false);
    }
}
