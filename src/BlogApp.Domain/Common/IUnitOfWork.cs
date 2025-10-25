namespace BlogApp.Domain.Common;

/// <summary>
/// Veritabanı transaction'larını yönetmek için Unit of Work pattern
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Bu context'te yapılan tüm değişiklikleri veritabanına kaydeder
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tüm değişiklikleri senkron olarak kaydeder
    /// </summary>
    int SaveChanges();

    /// <summary>
    /// Bir veritabanı transaction'ı başlatır
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut transaction'ı commit eder
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Mevcut transaction'ı geri alır
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Takip edilen entity'lerden tüm domain event'leri getirir
    /// </summary>
    IEnumerable<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Takip edilen entity'lerden tüm domain event'leri temizler
    /// </summary>
    void ClearDomainEvents();
}
