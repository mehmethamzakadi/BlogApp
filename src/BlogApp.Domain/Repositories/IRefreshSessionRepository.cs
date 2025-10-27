using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace BlogApp.Domain.Repositories;

public interface IRefreshSessionRepository : IAsyncRepository<RefreshSession>, IRepository<RefreshSession>
{
    Task<RefreshSession?> GetByTokenHashAsync(string tokenHash, bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshSession>> GetActiveSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
