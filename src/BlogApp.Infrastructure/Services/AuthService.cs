using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Repositories;
using BlogApp.Domain.Services;
using BlogApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BlogApp.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserDomainService _userDomainService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshSessionRepository _refreshSessionRepository;
    private readonly IMailService _mailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IUserDomainService userDomainService,
        ITokenService tokenService,
        IRefreshSessionRepository refreshSessionRepository,
        IMailService mailService,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userDomainService = userDomainService;
        _tokenService = tokenService;
        _refreshSessionRepository = refreshSessionRepository;
        _mailService = mailService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<IDataResult<LoginResponse>> LoginAsync(string email, string password, string? deviceId = null)
    {
        User? user = await _userRepository.FindByEmailAsync(email)
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
        if (!_userDomainService.VerifyPassword(user, password))
        {
            // Increment failed access count
            user.AccessFailedCount++;

            // Lock account after 5 failed attempts
            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(15);
            }

            _userRepository.Update(user);
            await SaveChangesWithConcurrencyHandlingAsync();

            throw new AuthenticationErrorException();
        }

        // Reset failed access count on successful login
        user.AccessFailedCount = 0;

        var authClaims = await _tokenService.GetAuthClaims(user);
        var accessToken = _tokenService.CreateAccessToken(authClaims, user);
        var refreshToken = _tokenService.CreateRefreshToken();

        _userRepository.Update(user);

        var session = new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Jti = accessToken.Jti,
            TokenHash = HashRefreshToken(refreshToken.Token),
            DeviceId = deviceId,
            ExpiresAt = refreshToken.ExpiresAt,
            Revoked = false,
            CreatedDate = DateTime.UtcNow,
            CreatedById = SystemUsers.SystemUserId
        };

        await _refreshSessionRepository.AddAsync(session);
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
        var session = await _refreshSessionRepository.GetByTokenHashAsync(tokenHash, includeDeleted: true);

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

        var user = await _userRepository.FindByIdAsync(session.UserId)
            ?? throw new AuthenticationErrorException("Kullanıcı bulunamadı.");

        if (user.IsLockedOut())
        {
            throw new AuthenticationErrorException("Hesabınız kilitlenmiş.");
        }

        var claims = await _tokenService.GetAuthClaims(user);
        var newAccess = _tokenService.CreateAccessToken(claims, user);
        var newRefresh = _tokenService.CreateRefreshToken();

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

        await _refreshSessionRepository.AddAsync(replacement);
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
        var session = await _refreshSessionRepository.GetByTokenHashAsync(tokenHash);
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
        User? user = await _userRepository.FindByEmailAsync(email);
        if (user != null)
        {
            // Rastgele token oluştur
            string resetToken = _passwordHasher.GeneratePasswordResetToken();

            // Token'ı hash'le ve veritabanına hash'i sakla
            string tokenHash = HashPasswordResetToken(resetToken);
            user.PasswordResetToken = tokenHash;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            _userRepository.Update(user);
            await SaveChangesWithConcurrencyHandlingAsync();
            // Kullanıcıya orijinal token'ı gönder (hash'i değil!)
            await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken.UrlEncode());
        }
    }

    private async Task SaveChangesWithConcurrencyHandlingAsync()
    {
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Tekrarlanan refresh token isteklerinde oluşabilecek çakışmaları yakalayıp kullanıcıyı yeniden girişe yönlendiriyoruz.
            throw new AuthenticationErrorException("Oturum verileriniz başka bir işlem tarafından güncellendi. Lütfen tekrar giriş yapın.", ex);
        }
    }

    private async Task RevokeAllSessionsAsync(Guid userId, string reason)
    {
        var sessions = await _refreshSessionRepository.GetActiveSessionsAsync(userId);
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

        User? user = await _userRepository.FindByIdAsync(userIdGuid);
        if (user != null && user.PasswordResetToken != null && user.PasswordResetTokenExpiry != null)
        {
            resetToken = resetToken.UrlDecode();
            string tokenHash = HashPasswordResetToken(resetToken);
            var storedTokenHash = user.PasswordResetToken;

            if (storedTokenHash == tokenHash && user.PasswordResetTokenExpiry > DateTime.UtcNow)
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

    private static string HashPasswordResetToken(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
