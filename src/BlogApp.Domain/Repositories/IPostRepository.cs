using System.Linq;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

/// <summary>
/// Post repository interface - specific queries to avoid IQueryable leaks
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    /// <summary>
    /// Get post by ID with category information
    /// </summary>
    Task<Post?> GetByIdWithCategoryAsync(Guid id, bool includeUnpublished = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get post by ID with projection to DTO (performance optimized)
    /// </summary>
    Task<TResult?> GetByIdProjectedAsync<TResult>(
        Guid id, 
        bool includeUnpublished,
        Func<IQueryable<Post>, IQueryable<TResult>> projection,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get published posts with pagination and optional category filter
    /// </summary>
    Task<Paginate<Post>> GetPublishedPostsAsync(
        Guid? categoryId = null,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get published posts with projection (performance optimized)
    /// </summary>
    Task<Paginate<TResult>> GetPublishedPostsProjectedAsync<TResult>(
        Func<IQueryable<Post>, IQueryable<TResult>> projection,
        Guid? categoryId = null,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Count total posts
    /// </summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Count published posts
    /// </summary>
    Task<int> CountPublishedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Count posts created after specified date
    /// </summary>
    Task<int> CountCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category has any active posts
    /// </summary>
    Task<bool> HasActivePostsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
