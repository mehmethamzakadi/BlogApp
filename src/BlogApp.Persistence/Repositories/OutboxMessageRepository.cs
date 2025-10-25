using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public class OutboxMessageRepository : EfRepositoryBase<OutboxMessage, BlogAppDbContext>, IOutboxMessageRepository
{
    public OutboxMessageRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 50, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(m => m.ProcessedAt == null && (m.NextRetryAt == null || m.NextRetryAt <= DateTime.UtcNow))
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OutboxMessage>> GetMessagesForRetryAsync(int maxRetryCount = 5, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(m => m.ProcessedAt == null
                        && m.Error != null
                        && m.RetryCount < maxRetryCount
                        && m.NextRetryAt <= DateTime.UtcNow)
            .OrderBy(m => m.NextRetryAt)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsProcessedAsync(int messageId, CancellationToken cancellationToken = default)
    {
        var message = await GetAsync(m => m.Id == messageId, cancellationToken: cancellationToken);
        if (message != null)
        {
            message.ProcessedAt = DateTime.UtcNow;
            message.Error = null;
            await UpdateAsync(message);
        }
    }

    public async Task MarkAsFailedAsync(int messageId, string error, DateTime? nextRetryAt = null, CancellationToken cancellationToken = default)
    {
        var message = await GetAsync(m => m.Id == messageId, cancellationToken: cancellationToken);
        if (message != null)
        {
            message.Error = error;
            message.RetryCount++;
            message.NextRetryAt = nextRetryAt ?? CalculateNextRetryTime(message.RetryCount);
            await UpdateAsync(message);
        }
    }

    public async Task CleanupProcessedMessagesAsync(int retentionDays = 7, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        var oldMessages = await Query()
            .Where(m => m.ProcessedAt != null && m.ProcessedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        foreach (var message in oldMessages)
        {
            await DeleteAsync(message, permanent: true);
        }
    }

    private static DateTime CalculateNextRetryTime(int retryCount)
    {
        // Exponential backoff: 1min, 2min, 4min, 8min, 16min
        var delayMinutes = Math.Pow(2, retryCount);
        return DateTime.UtcNow.AddMinutes(delayMinutes);
    }
}
