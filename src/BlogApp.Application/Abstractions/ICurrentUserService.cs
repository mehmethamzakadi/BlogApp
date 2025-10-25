namespace BlogApp.Application.Abstractions;

/// <summary>
/// Provides access to the current authenticated user's information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current authenticated user's ID
    /// </summary>
    /// <returns>User ID if authenticated, otherwise null</returns>
    int? GetCurrentUserId();
}
