using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// Category repository - specific query implementations to prevent IQueryable leaks
/// </summary>
public class CategoryRepository(BlogAppDbContext dbContext) : EfRepositoryBase<Category, BlogAppDbContext>(dbContext), ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(c => !c.IsDeleted)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Query().CountAsync(cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(c => c.ParentId == categoryId && !c.IsDeleted, cancellationToken);
    }

    public async Task<List<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(c => !c.IsDeleted && c.ParentId == null)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}
