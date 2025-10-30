using BlogApp.Domain.Common;
using BlogApp.Domain.Events.BookshelfItemEvents;

namespace BlogApp.Domain.Entities;

public sealed class BookshelfItem : BaseEntity
{
    public BookshelfItem() { }

    public string Title { get; set; } = default!;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int? PageCount { get; set; }
    public bool IsRead { get; set; }
    public string? Notes { get; set; }
    public DateTime? ReadDate { get; set; }
    public string? ImageUrl { get; set; }

    public static BookshelfItem Create(string title, string? author = null, string? publisher = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var item = new BookshelfItem
        {
            Title = title,
            Author = author,
            Publisher = publisher,
            IsRead = false
        };

        item.AddDomainEvent(new BookshelfItemCreatedEvent(item.Id, title, item.CreatedById ?? Guid.Empty));
        return item;
    }

    public void Update(string title, string? author = null, string? publisher = null, int? pageCount = null, string? notes = null, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
        Author = author;
        Publisher = publisher;
        PageCount = pageCount;
        Notes = notes;
        ImageUrl = imageUrl;

        AddDomainEvent(new BookshelfItemUpdatedEvent(Id, title, UpdatedById ?? Guid.Empty));
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("BookshelfItem is already deleted");

        AddDomainEvent(new BookshelfItemDeletedEvent(Id, Title, UpdatedById ?? Guid.Empty));
    }
}
