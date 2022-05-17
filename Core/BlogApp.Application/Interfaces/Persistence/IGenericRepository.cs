using BlogApp.Domain.Common;
using System.Linq.Expressions;

namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> GetWhere(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task<bool> Exists(int id);
        Task Update(T entity);
        Task Remove(T entity);
    }
}
