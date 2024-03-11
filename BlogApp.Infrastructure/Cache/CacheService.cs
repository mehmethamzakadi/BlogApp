﻿using BlogApp.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace BlogApp.Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            var result = await _distributedCache.GetAsync(key);
            if (result is not null)
            {
                var json = Encoding.UTF8.GetString(result);
                var response = JsonConvert.DeserializeObject<T>(json);
                return response;
            }
            return default;
        }

        public async Task RemoveDataAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var json = JsonConvert.SerializeObject(value);
            var valueFromCache = Encoding.UTF8.GetBytes(json);
            var options = new DistributedCacheEntryOptions()
                    //.SetSlidingExpiration(TimeSpan.FromDays(1)) // belirli bir süre erişilmemiş ise expire eder
                    .SetAbsoluteExpiration(expirationTime); // belirli bir süre sonra expire eder.
            await _distributedCache.SetAsync(key, valueFromCache, options);

            return true;
        }
    }
}
