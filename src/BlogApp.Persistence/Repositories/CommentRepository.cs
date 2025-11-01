using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// Comment repository implementation
/// ✅ DDD: Comment aggregate with specific query methods
/// </summary>
public class CommentRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Comment, BlogAppDbContext>(dbContext), ICommentRepository
{
    public async Task<Paginate<Comment>> GetCommentsByPostIdAsync(
        Guid postId,
        bool includeUnpublished = false,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .Where(c => c.PostId == postId)
            .AsNoTracking();

        if (!includeUnpublished)
        {
            query = query.Where(c => c.IsPublished);
        }

        query = query.OrderByDescending(c => c.CreatedDate);

        return await query.ToPaginateAsync(pageIndex, pageSize, cancellationToken);
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(c => c.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountByPostIdAsync(Guid postId, bool includeUnpublished = false, CancellationToken cancellationToken = default)
    {
        var query = Query().Where(c => c.PostId == postId);

        if (!includeUnpublished)
        {
            query = query.Where(c => c.IsPublished);
        }

        return await query.CountAsync(cancellationToken);
    }
}
