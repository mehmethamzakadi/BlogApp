using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IOutboxMessageRepository : IAsyncRepository<OutboxMessage>, IRepository<OutboxMessage>
{
    /// <summary>
    /// Get unprocessed messages ready for processing
    /// </summary>
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get messages that need retry (failed with retry count < max)
    /// </summary>
    Task<List<OutboxMessage>> GetMessagesForRetryAsync(int maxRetryCount = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark message as processed
    /// </summary>
    Task MarkAsProcessedAsync(int messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark message as failed with error details
    /// </summary>
    Task MarkAsFailedAsync(int messageId, string error, DateTime? nextRetryAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clean up old processed messages (retention policy)
    /// </summary>
    Task CleanupProcessedMessagesAsync(int retentionDays = 7, CancellationToken cancellationToken = default);
}
