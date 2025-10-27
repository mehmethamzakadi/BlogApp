using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlogApp.Domain.Common.Dynamic;

namespace BlogApp.Application.Common.Caching;

/// <summary>
/// Centers cache key conventions for frequently used resources so that invalidation stays consistent.
/// </summary>
public static class CacheKeys
{
    public static string Category(Guid categoryId) => $"category:{categoryId}";

    public static string PostPublic(Guid postId) => $"post:public:{postId}";

    public static string PostWithDrafts(Guid postId) => $"post:full:{postId}";

    public static string CategoryGridVersion() => "category:grid:version";

    public static string CategoryGrid(string versionToken, int pageIndex, int pageSize, DynamicQuery? dynamicQuery)
    {
        string dynamicSegment = dynamicQuery is null
            ? "none"
            : ComputeHash(dynamicQuery);

        return $"category:grid:{versionToken}:{pageIndex}:{pageSize}:{dynamicSegment}";
    }

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
