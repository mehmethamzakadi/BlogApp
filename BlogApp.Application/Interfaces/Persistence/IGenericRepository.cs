using BlogApp.Domain.Common;
using System.Linq.Expressions;

namespace BlogApp.Application.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : class, new()
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        IQueryable<T> GetWhere(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task AddAsyncRange(List<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        void Update(T entity);
        void UpdateRange(List<T> entites);
        void Remove(T entity);
        void RemoveRange(List<T> entites);
    }
}
