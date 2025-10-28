using System.Security.Claims;
using System.Threading;
using BlogApp.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Infrastructure.Services;

/// <summary>
/// IHttpContextAccessor tabanlı varsayılan yürütme bağlamı sağlayıcısı. Arka plan işler gibi
/// HttpContext'in bulunmadığı senaryolar için AsyncLocal üzerinden geçici kullanıcı kimliği atamaya
/// izin verir.
/// </summary>
public sealed class ExecutionContextAccessor(IHttpContextAccessor httpContextAccessor) : IExecutionContextAccessor
{
    private static readonly AsyncLocal<Guid?> CurrentUserOverride = new();

    public Guid? GetCurrentUserId()
    {
        var overrideValue = CurrentUserOverride.Value;
        if (overrideValue.HasValue)
        {
            return overrideValue;
        }

        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is not null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }

    public IDisposable BeginScope(Guid userId)
    {
        var previous = CurrentUserOverride.Value;
        CurrentUserOverride.Value = userId;
        return new RevertScope(() => CurrentUserOverride.Value = previous);
    }

    private sealed class RevertScope(Action revertAction) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            revertAction();
            _disposed = true;
        }
    }
}
