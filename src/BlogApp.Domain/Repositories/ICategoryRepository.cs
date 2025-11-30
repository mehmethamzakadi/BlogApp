using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

/// <summary>
/// Category repository interface - specific queries to avoid IQueryable leaks
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// Get category by ID
    /// </summary>
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active categories
    /// </summary>
    Task<List<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Count total categories
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category has any child categories
    /// </summary>
    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all root categories (categories without parent)
    /// </summary>
    Task<List<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
}
