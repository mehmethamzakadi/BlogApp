using BlogApp.Domain.Common.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BlogApp.Application.Common.Caching;

/// <summary>
/// Centers cache key conventions for frequently used resources so that invalidation stays consistent.
/// Uses version-based cache invalidation strategy to efficiently invalidate all related cache entries.
/// </summary>
public static class CacheKeys
{
    #region Single Entity Keys
    
    public static string Category(Guid categoryId) => $"category:{categoryId}";

    public static string Post(Guid postId) => $"post:{postId}";

    public static string PostPublic(Guid postId) => $"post:public:{postId}";

    public static string PostWithDrafts(Guid postId) => $"post:full:{postId}";
    
    #endregion

    #region Version Keys (for efficient cache invalidation)
    
    /// <summary>
    /// Version key for category grid. When this changes, all category grid caches become stale.
    /// </summary>
    public static string CategoryGridVersion() => "version:category:grid";

    /// <summary>
    /// Version key for all post lists. When this changes, all post list caches become stale.
    /// </summary>
    public static string PostListVersion() => "version:posts:list";

    /// <summary>
    /// Version key for posts in a specific category. When this changes, that category's post list caches become stale.
    /// </summary>
    public static string PostsByCategoryVersion(Guid categoryId) => $"version:posts:category:{categoryId}";
    
    #endregion

    #region Versioned List Keys
    
    /// <summary>
    /// Cache key for paginated post list with version token.
    /// Version changes invalidate all cached pages automatically.
    /// </summary>
    public static string PostList(string versionToken, int pageIndex, int pageSize) 
        => $"posts:list:{versionToken}:{pageIndex}:{pageSize}";

    /// <summary>
    /// Cache key for paginated posts by category with version token.
    /// Version changes invalidate all cached pages automatically.
    /// </summary>
    public static string PostsByCategory(string versionToken, Guid categoryId, int pageIndex, int pageSize) 
        => $"posts:category:{categoryId}:{versionToken}:{pageIndex}:{pageSize}";

    /// <summary>
    /// Cache key for category grid with version token and dynamic query support.
    /// </summary>
    public static string CategoryGrid(string versionToken, int pageIndex, int pageSize, DynamicQuery? dynamicQuery)
    {
        string dynamicSegment = dynamicQuery is null
            ? "none"
            : ComputeHash(dynamicQuery);

        return $"category:grid:{versionToken}:{pageIndex}:{pageSize}:{dynamicSegment}";
    }
    
    #endregion

    #region Legacy Keys (deprecated - use versioned keys instead)
    
    [Obsolete("Use PostList(versionToken, pageIndex, pageSize) instead for version-based invalidation")]
    public static string PostListLegacy(int pageIndex, int pageSize) => $"posts:list:{pageIndex}:{pageSize}";

    [Obsolete("Use PostsByCategory(versionToken, categoryId, pageIndex, pageSize) instead for version-based invalidation")]
    public static string PostsByCategoryLegacy(Guid categoryId, int pageIndex, int pageSize) => $"posts:category:{categoryId}:{pageIndex}:{pageSize}";
    
    #endregion

    #region Helpers
    
    private static readonly JsonSerializerOptions KeySerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static string ComputeHash<T>(T value)
    {
        string json = JsonSerializer.Serialize(value, KeySerializerOptions);
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }
    
    #endregion
}

/// <summary>
/// Provides TTL recommendations for cache entries. Keep the values conservative until usage patterns are validated.
/// </summary>
public static class CacheDurations
{
    public static readonly TimeSpan Category = TimeSpan.FromHours(12);

    public static readonly TimeSpan Post = TimeSpan.FromMinutes(30);

    public static readonly TimeSpan CategoryGrid = TimeSpan.FromDays(30);
}
