using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class Image : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Title { get; set; }
    public int? Size { get; set; }
    public string Path { get; set; } = default!;
    public string Type { get; set; } = default!;
}
