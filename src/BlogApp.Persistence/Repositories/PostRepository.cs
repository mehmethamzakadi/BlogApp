using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// Post repository - specific query implementations to prevent IQueryable leaks
/// </summary>
public class PostRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Post, BlogAppDbContext>(dbContext), IPostRepository
{
    public async Task<Post?> GetByIdWithCategoryAsync(Guid id, bool includeUnpublished = false, CancellationToken cancellationToken = default)
    {
        var query = Query()
            .Include(p => p.Category)
            .AsNoTracking()
            .Where(p => p.Id == id);

        if (!includeUnpublished)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// ✅ PERFORMANCE: Get post with projection to avoid loading full entity
    /// </summary>
    public async Task<TResult?> GetByIdProjectedAsync<TResult>(
        Guid id,
        bool includeUnpublished,
        Func<IQueryable<Post>, IQueryable<TResult>> projection,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .AsNoTracking()
            .Where(p => p.Id == id);

        if (!includeUnpublished)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await projection(query).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Paginate<Post>> GetPublishedPostsAsync(
        Guid? categoryId = null,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .Where(p => p.IsPublished)
            .Include(p => p.Category)
            .AsNoTracking();

        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        query = query.OrderByDescending(p => p.CreatedDate);

        return await query.ToPaginateAsync(pageIndex, pageSize, cancellationToken);
    }

    /// <summary>
    /// ✅ PERFORMANCE: Get published posts with projection to avoid loading full entities
    /// </summary>
    public async Task<Paginate<TResult>> GetPublishedPostsProjectedAsync<TResult>(
        Func<IQueryable<Post>, IQueryable<TResult>> projection,
        Guid? categoryId = null,
        int pageIndex = 0,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = Query()
            .Where(p => p.IsPublished)
            .AsNoTracking();

        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        query = query.OrderByDescending(p => p.CreatedDate);

        return await projection(query).ToPaginateAsync(pageIndex, pageSize, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Query().CountAsync(cancellationToken);
    }

    public async Task<int> CountPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.IsPublished)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.CreatedDate >= date)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> HasActivePostsInCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted, cancellationToken);
    }
}
