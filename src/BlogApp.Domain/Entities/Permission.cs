using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Sistemdeki permission'ları temsil eder.
/// Her permission bir modül ve aksiyon kombinasyonudur (örn: Users.Create, Posts.Delete)
/// </summary>
public sealed class Permission : BaseEntity
{
    /// <summary>
    /// Permission'ın benzersiz adı (örn: "Users.Create", "Posts.Delete")
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Normalize edilmiş permission adı (case-insensitive arama için)
    /// </summary>
    public string? NormalizedName { get; private set; }

    /// <summary>
    /// Permission'ın açıklaması
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Hangi modüle ait olduğu (Users, Posts, Categories vb.)
    /// </summary>
    public string Module { get; private set; } = default!;

    /// <summary>
    /// Permission tipi (Create, Read, Update, Delete vb.)
    /// </summary>
    public string Type { get; private set; } = default!;

    /// <summary>
    /// Bu permission'a sahip roller
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public static Permission Create(string name, string module, string type, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(module))
            throw new ArgumentException("Module cannot be empty", nameof(module));
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Type cannot be empty", nameof(type));

        var permission = new Permission
        {
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            Module = module,
            Type = type,
            Description = description
        };

        return permission;
    }
}
