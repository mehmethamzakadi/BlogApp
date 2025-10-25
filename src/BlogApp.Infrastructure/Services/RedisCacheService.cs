using BlogApp.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BlogApp.Infrastructure.Services;

public sealed class RedisCacheService(IDistributedCache distributedCache) : ICacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    public async Task Add(string key, object data, DateTimeOffset? absExpr, TimeSpan? sldExpr)
    {
        if (data is null)
        {
            return;
        }

        var cacheEntryOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = absExpr,
            SlidingExpiration = sldExpr
        };

        string json = JsonSerializer.Serialize(data, SerializerOptions);
        await distributedCache.SetStringAsync(key, json, cacheEntryOptions);
    }

    public bool Any(string key)
    {
        return !string.IsNullOrEmpty(distributedCache.GetString(key));
    }

    public async Task<T?> Get<T>(string key)
    {
        var data = await distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(data, SerializerOptions);
    }

    public async Task Remove(string key)
    {
        await distributedCache.RemoveAsync(key);
    }
}
