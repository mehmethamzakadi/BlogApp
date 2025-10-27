using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Represents a persisted refresh token session for a user/device pair.
/// </summary>
public sealed class RefreshSession : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid Jti { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string? DeviceId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    public Guid? ReplacedById { get; set; }
}
