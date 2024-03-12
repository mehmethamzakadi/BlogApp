using BlogApp.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace BlogApp.Infrastructure.Cache
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        public async Task Add(string key, object data, DateTimeOffset? absExpr, TimeSpan? sldExpr)
        {
            var json = JsonConvert.SerializeObject(data);

            await distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions() { AbsoluteExpiration = absExpr, SlidingExpiration = sldExpr });
        }
        public async Task Add(string key, byte[] data, DateTimeOffset? absExpr, TimeSpan? sldExpr)
        {
            await distributedCache.SetAsync(key, data, new DistributedCacheEntryOptions() { AbsoluteExpiration = absExpr, SlidingExpiration = sldExpr });
        }
        public bool Any(string key)
        {
            return !string.IsNullOrEmpty(distributedCache.GetString(key));
        }

        public async Task<T?> Get<T>(string key)
        {
            if (Any(key))
            {
                var data = await distributedCache.GetStringAsync(key) ?? string.Empty;
                return JsonConvert.DeserializeObject<T>(data);
            }
            return default;
        }

        public async Task Remove(string key)
        {
            await distributedCache.RemoveAsync(key);
        }
    }
}
