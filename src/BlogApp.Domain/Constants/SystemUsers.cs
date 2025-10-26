using System;

namespace BlogApp.Domain.Constants;

/// <summary>
/// Application-wide identifiers for synthetic or system-level users.
/// </summary>
public static class SystemUsers
{
    /// <summary>
    /// Identifier used when operations are executed without an authenticated user context.
    /// </summary>
    public static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
}
