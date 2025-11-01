using System.Linq;

namespace BlogApp.Domain.Common;

/// <summary>
/// Query operasyonları için repository interface
/// CQRS pattern'de read-side için kullanılır
/// </summary>
public interface IQueryRepository<TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Query();
}
