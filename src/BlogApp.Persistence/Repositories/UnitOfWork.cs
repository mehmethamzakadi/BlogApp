using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Attributes;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// BlogAppDbContext için Unit of Work implementasyonu
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly BlogAppDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(BlogAppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // OUTBOX PATTERN UYGULAMASI
            // Kaydetmeden önce takip edilen entity'lerden domain event'leri al
            var domainEvents = GetDomainEvents().ToList();

            // Domain event'leri RabbitMQ üzerinden asenkron işleme için outbox mesajlarına dönüştür
            // Bu, ACID garantilerini sağlar - event'ler business data ile aynı transaction içinde saklanır
            foreach (var domainEvent in domainEvents)
            {
                if (ShouldStoreInOutbox(domainEvent))
                {
                    var outboxMessage = new OutboxMessage
                    {
                        EventType = domainEvent.GetType().Name,
                        Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                        CreatedAt = DateTime.UtcNow,
                        RetryCount = 0
                    };

                    await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
                }
            }

            // Her şeyi tek bir atomik transaction içinde kaydet:
            // - Business data (entity'ler)
            // - Outbox mesajları (event'ler)
            var result = await _context.SaveChangesAsync(cancellationToken);

            // Başarılı kayıttan sonra domain event'leri temizle
            // Event'ler artık Outbox tablosunda güvenle saklanıyor
            ClearDomainEvents();

            return result;
        }
        finally
        {
            // ✅ DÜZELTİLDİ: SaveChanges başarısız olsa bile domain event'lerin her zaman temizlenmesini sağla
            // Bu, bellek sızıntılarını ve eski event'lerin yeniden işlenmesini önler
            ClearDomainEvents();
        }
    }

    private static bool ShouldStoreInOutbox(IDomainEvent domainEvent)
    {
        // ✅ DÜZELTİLDİ: Magic string'ler yerine attribute kullanan tip güvenli yaklaşım
        // Domain event tipinin [StoreInOutbox] attribute'una sahip olup olmadığını kontrol et
        // Bu, derleme zamanı güvenliği ve daha kolay refactoring sağlar
        var eventType = domainEvent.GetType();
        return eventType.GetCustomAttributes(typeof(StoreInOutboxAttribute), false).Any();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        // BaseEntity'den türeyen entity'lerin event'lerini al
        var baseEntityEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents);

        // IHasDomainEvents interface'ini implement eden entity'lerin event'lerini al
        // (örn: AppUser, AppRole - bunlar BaseEntity'den türemiyor ama domain event'e sahip)
        var hasDomainEventsEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .Cast<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IHasDomainEvents>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents);

        return baseEntityEvents.Concat(hasDomainEventsEntities).ToList();
    }

    public void ClearDomainEvents()
    {
        // BaseEntity'den türeyen entity'lerin event'lerini temizle
        var baseEntities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in baseEntities)
        {
            entity.ClearDomainEvents();
        }

        // IHasDomainEvents interface'ini implement eden entity'lerin event'lerini temizle
        var hasDomainEventsEntities = _context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .Cast<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<IHasDomainEvents>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in hasDomainEventsEntities)
        {
            entity.ClearDomainEvents();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }
}
