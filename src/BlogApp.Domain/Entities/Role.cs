using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki rolleri temsil eder.
/// Identity'den bağımsız, custom role entity.
/// </summary>
public sealed class Role : BaseEntity
{
    /// <summary>
    /// Rol adı (benzersiz olmalı, örn: "Admin", "User", "Moderator")
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Normalize edilmiş rol adı (case-insensitive arama için)
    /// </summary>
    public string NormalizedName { get; private set; } = default!;

    /// <summary>
    /// Rol açıklaması
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Concurrency stamp - optimistic concurrency için
    /// </summary>
    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Bu role atanmış permission'lar
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// Bu role sahip kullanıcılar
    /// </summary>
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    public static Role Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        var role = new Role
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Description = description
        };

        role.AddDomainEvent(new Domain.Events.RoleEvents.RoleCreatedEvent(role.Id, name));
        return role;
    }

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        ConcurrencyStamp = Guid.NewGuid().ToString(); // Update concurrency stamp on modification

        AddDomainEvent(new Domain.Events.RoleEvents.RoleUpdatedEvent(Id, name));
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Role is already deleted");

        AddDomainEvent(new Domain.Events.RoleEvents.RoleDeletedEvent(Id, Name));
    }
}