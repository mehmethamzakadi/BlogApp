using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;


namespace BlogApp.Domain.Repositories;

public interface IActivityLogRepository
{
    Task<ActivityLog> AddAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);
    Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<Paginate<ActivityLog>> GetPaginatedListByDynamicAsync(
        DynamicQuery dynamic,
        int index = 0,
        int size = 10,
        Func<System.Linq.IQueryable<ActivityLog>, IQueryable<ActivityLog>>? include = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Idempotency kontrolü için - belirli bir ID'ye sahip ActivityLog var mı?
    /// </summary>
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
