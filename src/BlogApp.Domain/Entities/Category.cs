using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = default!;

    /// <summary>
    /// Normalize edilmiş kategori adı (case-insensitive arama için)
    /// </summary>
    public string? NormalizedName { get; set; }
}
