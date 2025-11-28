namespace BlogApp.Domain.Common;

/// <summary>
/// Interface for entities that support soft delete.
/// Entities implementing this interface will have IsDeleted filter applied automatically.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Indicates whether the entity is soft deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// The date when the entity was soft deleted.
    /// </summary>
    DateTime? DeletedDate { get; set; }
}
