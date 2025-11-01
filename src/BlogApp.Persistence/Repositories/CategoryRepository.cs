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
}
