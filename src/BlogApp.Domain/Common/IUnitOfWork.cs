using System.Collections.Generic;

namespace BlogApp.Domain.Common;

/// <summary>
/// Unit of Work pattern for managing database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes synchronously
    /// </summary>
    int SaveChanges();

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all domain events from tracked entities
    /// </summary>
    IEnumerable<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all domain events from tracked entities
    /// </summary>
    void ClearDomainEvents();
}
