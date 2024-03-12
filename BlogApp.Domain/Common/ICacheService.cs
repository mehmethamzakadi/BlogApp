using System;
using System.Threading.Tasks;

namespace BlogApp.Domain.Common
{
    public interface ICacheService
    {
        Task<T?> Get<T>(string key);
        Task Add(string key, object data, DateTimeOffset? absExpr, TimeSpan? sldExpr);
        bool Any(string key);
        Task Remove(string key);
    }
}
