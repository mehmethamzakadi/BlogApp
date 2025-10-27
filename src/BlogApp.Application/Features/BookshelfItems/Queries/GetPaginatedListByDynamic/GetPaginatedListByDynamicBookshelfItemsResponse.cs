using BlogApp.Application.Common;

namespace BlogApp.Application.Features.BookshelfItems.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicBookshelfItemsResponse : BaseEntityResponse
{
    public string Title { get; init; } = string.Empty;
    public string? Author { get; init; }
    public string? Publisher { get; init; }
    public int? PageCount { get; init; }
    public bool IsRead { get; init; }
    public string? Notes { get; init; }
    public DateTime? ReadDate { get; init; }
    public string? ImageUrl { get; init; }
}
