using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace BlogApp.Persistence.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly BlogAppDbContext _context;

    public ActivityLogRepository(BlogAppDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityLog> AddAsync(ActivityLog activityLog, CancellationToken cancellationToken = default)
    {
        await _context.ActivityLogs.AddAsync(activityLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return activityLog;
    }

    public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.ActivityLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Paginate<ActivityLog>> GetPaginatedListByDynamicAsync(
        DynamicQuery dynamic,
        int index = 0,
        int size = 10,
        Func<IQueryable<ActivityLog>, IIncludableQueryable<ActivityLog, object>>? include = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ActivityLog> queryable = _context.ActivityLogs.AsQueryable();

        if (include != null)
            queryable = include(queryable);

        // Apply dynamic sorting
        if (dynamic.Sort != null && dynamic.Sort.Any())
        {
            foreach (var sort in dynamic.Sort)
            {
                queryable = sort.Dir == "asc"
                    ? queryable.OrderBy(a => EF.Property<object>(a, sort.Field))
                    : queryable.OrderByDescending(a => EF.Property<object>(a, sort.Field));
            }
        }

        var count = await queryable.CountAsync(cancellationToken);
        var items = await queryable
            .Skip(index * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new Paginate<ActivityLog>
        {
            Items = items,
            Index = index,
            Size = size,
            Count = count,
            Pages = (int)Math.Ceiling(count / (double)size)
        };
    }
}
