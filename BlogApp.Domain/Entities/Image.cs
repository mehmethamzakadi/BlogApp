using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public class Image : BaseEntity
{
    public string Name { get; set; }
    public string? Title { get; set; }
    public int? Size { get; set; }
    public string Path { get; set; }
    public string Type { get; set; }
}
