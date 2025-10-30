using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace BlogApp.Domain.Repositories;

public interface IActivityLogRepository
{
    Task<ActivityLog> AddAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);
    Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<Paginate<ActivityLog>> GetPaginatedListByDynamicAsync(
        DynamicQuery dynamic,
        int index = 0,
        int size = 10,
        Func<System.Linq.IQueryable<ActivityLog>, IIncludableQueryable<ActivityLog, object>>? include = null,
        CancellationToken cancellationToken = default);
}
