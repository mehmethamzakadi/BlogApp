using System;
using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class BookshelfItem : BaseEntity
{
    public string Title { get; set; } = default!;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int? PageCount { get; set; }
    public bool IsRead { get; set; }
    public string? Notes { get; set; }
    public DateTime? ReadDate { get; set; }
}
