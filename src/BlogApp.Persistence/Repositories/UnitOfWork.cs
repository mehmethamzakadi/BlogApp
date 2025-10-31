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
        var domainEvents = GetDomainEvents().ToList();

        if (!domainEvents.Any())
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
            {
                if (ShouldStoreInOutbox(domainEvent))
                {
                    var idempotencyKey = $"{domainEvent.GetType().Name}:{domainEvent.AggregateId}:{domainEvent.OccurredOn.Ticks}";

                    var outboxMessage = new OutboxMessage
                    {
                        IdempotencyKey = idempotencyKey,
                        EventType = domainEvent.GetType().Name,
                        Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                        CreatedAt = DateTime.UtcNow,
                        RetryCount = 0
                    };

                    await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            ClearDomainEvents();
        }
    }

    private static bool ShouldStoreInOutbox(IDomainEvent domainEvent)
    {
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
        return _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
    }

    public void ClearDomainEvents()
    {
        var entities = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
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