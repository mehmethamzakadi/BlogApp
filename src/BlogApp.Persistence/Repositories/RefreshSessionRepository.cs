using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public sealed class RefreshSessionRepository : EfRepositoryBase<RefreshSession, BlogAppDbContext>, IRefreshSessionRepository
{
    public RefreshSessionRepository(BlogAppDbContext context) : base(context)
    {
    }

    public async Task<RefreshSession?> GetByTokenHashAsync(string tokenHash, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        IQueryable<RefreshSession> query = Context.RefreshSessions.AsQueryable();

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshSession>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.RefreshSessions
            .Where(x => x.UserId == userId && !x.Revoked && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}
