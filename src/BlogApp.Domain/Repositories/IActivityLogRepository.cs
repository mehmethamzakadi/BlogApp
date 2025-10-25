using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using System.Collections.Generic;

namespace BlogApp.Domain.Repositories;

public interface IActivityLogRepository : IAsyncRepository<ActivityLog>, IRepository<ActivityLog>
{
    Task<List<ActivityLog>> GetRecentActivitiesAsync(int count = 10, CancellationToken cancellationToken = default);
}
