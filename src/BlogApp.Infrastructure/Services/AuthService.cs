using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Extentions;
using BlogApp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Infrastructure.Services.Identity;

public sealed class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IRefreshSessionRepository refreshSessionRepository,
    IMailService mailService,
    IPasswordHasher passwordHasher,
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
        var accessToken = tokenService.CreateAccessToken(authClaims, user);
        var refreshToken = tokenService.CreateRefreshToken();

        var successUpdate = await userRepository.UpdateAsync(user);
        if (!successUpdate.Success)
        {
            throw new AuthenticationErrorException(successUpdate.Message ?? "Kullanıcı güncelleme hatası.");
        }

        var session = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Jti = accessToken.Jti,
            TokenHash = HashRefreshToken(refreshToken.Token),
            DeviceId = null,
            ExpiresAt = refreshToken.ExpiresAt,
            Revoked = false,
            CreatedDate = DateTime.UtcNow,
            CreatedById = SystemUsers.SystemUserId
        };

        await refreshSessionRepository.AddAsync(session);
        await SaveChangesWithConcurrencyHandlingAsync();

        var response = new LoginResponse(
            user.Id,
            user.UserName,
            accessToken.ExpiresAt,
            accessToken.Token,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            accessToken.Permissions.ToList());

        return new SuccessDataResult<LoginResponse>(response, "Giriş Başarılı");
    }

    public async Task<IDataResult<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AuthenticationErrorException("Geçersiz refresh token.");
        }

        var tokenHash = HashRefreshToken(refreshToken);
        var session = await refreshSessionRepository.GetByTokenHashAsync(tokenHash, includeDeleted: true);

        if (session is null)
        {
            throw new AuthenticationErrorException("Geçersiz refresh token.");
        }

        if (session.Revoked)
        {
            session.RevokedReason ??= "Replay detected";
            session.UpdatedDate = DateTime.UtcNow;
            session.UpdatedById = SystemUsers.SystemUserId;
            await RevokeAllSessionsAsync(session.UserId, "Replay detected");
            await SaveChangesWithConcurrencyHandlingAsync();
            throw new AuthenticationErrorException("Refresh token kullanılamaz durumda.");
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            session.Revoked = true;
            session.RevokedAt = DateTime.UtcNow;
            session.RevokedReason = "Expired";
            session.UpdatedDate = DateTime.UtcNow;
            session.UpdatedById = SystemUsers.SystemUserId;
            await SaveChangesWithConcurrencyHandlingAsync();
            throw new AuthenticationErrorException("Refresh token süresi dolmuş.");
        }

        var user = await userRepository.FindByIdAsync(session.UserId)
            ?? throw new AuthenticationErrorException("Kullanıcı bulunamadı.");

        if (user.IsLockedOut())
        {
            throw new AuthenticationErrorException("Hesabınız kilitlenmiş.");
        }

        var claims = await tokenService.GetAuthClaims(user);
        var newAccess = tokenService.CreateAccessToken(claims, user);
        var newRefresh = tokenService.CreateRefreshToken();

        session.Revoked = true;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = "Rotated";
        session.UpdatedDate = DateTime.UtcNow;
        session.UpdatedById = SystemUsers.SystemUserId;

        var replacement = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Jti = newAccess.Jti,
            TokenHash = HashRefreshToken(newRefresh.Token),
            DeviceId = session.DeviceId,
            ExpiresAt = newRefresh.ExpiresAt,
            Revoked = false,
            CreatedDate = DateTime.UtcNow,
            CreatedById = SystemUsers.SystemUserId
        };

        session.ReplacedById = replacement.Id;

        await refreshSessionRepository.AddAsync(replacement);
        await SaveChangesWithConcurrencyHandlingAsync();

        var response = new LoginResponse(
            user.Id,
            user.UserName,
            newAccess.ExpiresAt,
            newAccess.Token,
            newRefresh.Token,
            newRefresh.ExpiresAt,
            newAccess.Permissions.ToList());

        return new SuccessDataResult<LoginResponse>(response, "Token yenilendi");
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var tokenHash = HashRefreshToken(refreshToken);
        var session = await refreshSessionRepository.GetByTokenHashAsync(tokenHash);
        if (session is null)
        {
            return;
        }

        session.Revoked = true;
        session.RevokedAt = DateTime.UtcNow;
        session.RevokedReason = "Logout";
        session.UpdatedDate = DateTime.UtcNow;
        session.UpdatedById = SystemUsers.SystemUserId;

        await SaveChangesWithConcurrencyHandlingAsync();
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

    private async Task RevokeAllSessionsAsync(Guid userId, string reason)
    {
        var sessions = await refreshSessionRepository.GetActiveSessionsAsync(userId);
        foreach (var session in sessions)
        {
            session.Revoked = true;
            session.RevokedAt = DateTime.UtcNow;
            session.RevokedReason = reason;
            session.UpdatedDate = DateTime.UtcNow;
            session.UpdatedById = SystemUsers.SystemUserId;
        }
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

    private static string HashRefreshToken(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
