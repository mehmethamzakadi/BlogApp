using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class Category : BaseEntity
{
    public string Name { get; set; } = default!;
}
