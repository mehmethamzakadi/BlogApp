using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// Unit of Work implementation for BlogAppDbContext
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
        // OUTBOX PATTERN IMPLEMENTATION
        // Get domain events from tracked entities before saving
        var domainEvents = GetDomainEvents().ToList();

        // Convert domain events to outbox messages for async processing via RabbitMQ
        // This ensures ACID guarantees - events are stored in the same transaction as business data
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

        // Save everything in one atomic transaction:
        // - Business data (entities)
        // - Outbox messages (events)
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Clear domain events after successful save
        // Events are now safely stored in Outbox table
        ClearDomainEvents();

        return result;
    }

    private static bool ShouldStoreInOutbox(IDomainEvent domainEvent)
    {
        // Determine which domain events should be processed asynchronously via Outbox Pattern
        // These events will be:
        // 1. Stored in OutboxMessages table (same transaction as business data)
        // 2. Picked up by OutboxProcessorService (background service)
        // 3. Published to RabbitMQ
        // 4. Consumed by ActivityLogConsumer
        // 5. Converted to ActivityLog records

        var eventTypeName = domainEvent.GetType().Name;

        // Events that create ActivityLog entries
        var outboxEventTypes = new[]
        {
            "CategoryCreatedEvent",
            "CategoryUpdatedEvent",
            "CategoryDeletedEvent",
            "PostCreatedEvent",
            "PostUpdatedEvent",
            "PostDeletedEvent",
            "UserCreatedEvent",
            "UserUpdatedEvent",
            "UserDeletedEvent",
            "UserRolesAssignedEvent",
            "RoleCreatedEvent",
            "RoleUpdatedEvent",
            "RoleDeletedEvent",
            "PermissionsAssignedToRoleEvent"
        };

        return outboxEventTypes.Contains(eventTypeName);
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
            .Select(e => e.Entity);

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
