using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common;
using BlogApp.Domain.Constants;
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
    BlogAppDbContext dbContext,
    IUnitOfWork unitOfWork) : IAuthService
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

            var failureUpdate = await userRepository.UpdateAsync(user);
            if (failureUpdate.Success)
            {
                await SaveChangesWithConcurrencyHandlingAsync();
            }

            throw new AuthenticationErrorException();
        }

        // Reset failed access count on successful login
        user.AccessFailedCount = 0;

        var authClaims = await tokenService.GetAuthClaims(user);
        var tokenResponse = tokenService.GenerateAccessToken(authClaims, user);

        // Store refresh token
        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        var successUpdate = await userRepository.UpdateAsync(user);
        if (!successUpdate.Success)
        {
            throw new AuthenticationErrorException(successUpdate.Message ?? "Kullanıcı güncelleme hatası.");
        }

        await SaveChangesWithConcurrencyHandlingAsync();

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

        var refreshExpiry = DateTime.UtcNow.AddDays(7);

        var rotationSucceeded = await TryRotateRefreshTokenAsync(user.Id, refreshToken, tokenResponse.RefreshToken, refreshExpiry);
        if (!rotationSucceeded)
        {
            throw new AuthenticationErrorException("Geçersiz veya süresi dolmuş refresh token.");
        }

        return new SuccessDataResult<LoginResponse>(tokenResponse, "Token yenilendi");
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
        {
            return;
        }

        await ClearRefreshTokenAsync(refreshToken);
    }

    public async Task PasswordResetAsync(string email)
    {
        User? user = await userRepository.FindByEmailAsync(email);
        if (user != null)
        {
            string resetToken = passwordHasher.GeneratePasswordResetToken();
            user.PasswordResetToken = resetToken.UrlEncode();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            var updateResult = await userRepository.UpdateAsync(user);
            if (updateResult.Success)
            {
                await SaveChangesWithConcurrencyHandlingAsync();
                await mailService.SendPasswordResetMailAsync(email, user.Id, resetToken.UrlEncode());
            }
        }
    }

    private async Task SaveChangesWithConcurrencyHandlingAsync()
    {
        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Tekrarlanan refresh token isteklerinde oluşabilecek çakışmaları yakalayıp kullanıcıyı yeniden girişe yönlendiriyoruz.
            throw new AuthenticationErrorException("Oturum verileriniz başka bir işlem tarafından güncellendi. Lütfen tekrar giriş yapın.", ex);
        }
    }

    private async Task<bool> TryRotateRefreshTokenAsync(Guid userId, string currentRefreshToken, string newRefreshToken, DateTime newExpiryUtc)
    {
        var nowUtc = DateTime.UtcNow;
        var newConcurrencyStamp = Guid.NewGuid().ToString();

        var affectedRows = await dbContext.Users
            .Where(u => u.Id == userId && u.RefreshToken == currentRefreshToken && !u.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.RefreshToken, _ => newRefreshToken)
                .SetProperty(u => u.RefreshTokenExpiry, _ => newExpiryUtc)
                .SetProperty(u => u.UpdatedDate, _ => nowUtc)
                .SetProperty(u => u.UpdatedById, _ => SystemUsers.SystemUserId)
                .SetProperty(u => u.ConcurrencyStamp, _ => newConcurrencyStamp));

        return affectedRows > 0;
    }

    private async Task ClearRefreshTokenAsync(string refreshToken)
    {
        var nowUtc = DateTime.UtcNow;
        var newConcurrencyStamp = Guid.NewGuid().ToString();

        await dbContext.Users
            .Where(u => u.RefreshToken == refreshToken && !u.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.RefreshToken, _ => (string?)null)
                .SetProperty(u => u.RefreshTokenExpiry, _ => (DateTime?)null)
                .SetProperty(u => u.UpdatedDate, _ => nowUtc)
                .SetProperty(u => u.UpdatedById, _ => SystemUsers.SystemUserId)
                .SetProperty(u => u.ConcurrencyStamp, _ => newConcurrencyStamp));
    }

    public async Task<IDataResult<bool>> PasswordVerify(string resetToken, string userId)
    {
        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            return new SuccessDataResult<bool>(false);
        }

        User? user = await userRepository.FindByIdAsync(userIdGuid);
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
