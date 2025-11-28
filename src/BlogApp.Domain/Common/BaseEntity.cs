using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Domain.Common;

/// <summary>
/// Tüm domain entity'leri için temel sınıf.
/// Ortak özellikleri ve domain event yönetimini sağlar.
/// ISoftDeletable: Soft delete desteği için. Global query filter sadece ISoftDeletable entity'lere uygulanır.
/// </summary>
public abstract class BaseEntity : IEntityTimestamps, IHasDomainEvents, ISoftDeletable
{
    public Guid Id { get; set; }

    /// <summary>
    /// Optimistic concurrency control için RowVersion
    /// Timestamp attribute EF Core tarafından otomatik yönetilir
    /// </summary>
    [System.ComponentModel.DataAnnotations.Timestamp]
    public byte[]? RowVersion { get; set; }

    /// <summary>
    /// Entity'nin oluşturulma tarihi
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Entity'yi oluşturan kullanıcının ID'si
    /// </summary>
    public Guid? CreatedById { get; set; }

    /// <summary>
    /// Entity'nin son güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Entity'yi son güncelleyen kullanıcının ID'si
    /// </summary>
    public Guid? UpdatedById { get; set; }

    /// <summary>
    /// Soft delete için işaretleme (fiziksel silme yerine)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Entity'nin silinme tarihi (IsDeleted = true ise)
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    // Domain Events - Entity üzerindeki önemli olayları takip eder
    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Entity'ye yeni bir domain event ekler
    /// </summary>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Entity'den bir domain event'i kaldırır
    /// </summary>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Tüm domain event'leri temizler (genellikle event'ler işlendikten sonra)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
