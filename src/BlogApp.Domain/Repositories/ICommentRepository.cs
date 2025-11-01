using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

/// <summary>
/// Comment repository interface
/// ✅ DDD: Comment is now a separate aggregate with its own repository
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Get comments by post ID with pagination
    /// </summary>
    Task<Paginate<Comment>> GetCommentsByPostIdAsync(
        Guid postId,
        bool includeUnpublished = false,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get comment by ID
    /// </summary>
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Count comments for a post
    /// </summary>
    Task<int> CountByPostIdAsync(Guid postId, bool includeUnpublished = false, CancellationToken cancellationToken = default);
}
