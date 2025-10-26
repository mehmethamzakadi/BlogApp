using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;

namespace BlogApp.Domain.Repositories;

public interface IOutboxMessageRepository : IAsyncRepository<OutboxMessage>, IRepository<OutboxMessage>
{
    /// <summary>
    /// İşlenmeye hazır, işlenmemiş mesajları getirir
    /// </summary>
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Yeniden denenmesi gereken mesajları getirir (başarısız ve retry sayısı < max)
    /// </summary>
    Task<List<OutboxMessage>> GetMessagesForRetryAsync(int maxRetryCount = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mesajı işlenmiş olarak işaretle
    /// </summary>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mesajı hata detaylarıyla birlikte başarısız olarak işaretle
    /// </summary>
    Task MarkAsFailedAsync(Guid messageId, string error, DateTime? nextRetryAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eski işlenmiş mesajları temizle (saklama politikası)
    /// </summary>
    Task CleanupProcessedMessagesAsync(int retentionDays = 7, CancellationToken cancellationToken = default);
}
