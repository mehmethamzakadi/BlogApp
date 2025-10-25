using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlogApp.Persistence.Repositories;

public class ActivityLogRepository : EfRepositoryBase<ActivityLog, BlogAppDbContext>, IActivityLogRepository
{
    public ActivityLogRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
