using BlogApp.Domain.Common;
using BlogApp.Domain.Events.BookshelfItemEvents;

namespace BlogApp.Domain.Entities;

public sealed class BookshelfItem : BaseEntity
{
    // EF Core için parameterless constructor
    public BookshelfItem() { }

    public string Title { get; private set; } = default!;
    public string? Author { get; private set; }
    public string? Publisher { get; private set; }
    public int? PageCount { get; private set; }
    public bool IsRead { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? ReadDate { get; private set; }
    public string? ImageUrl { get; private set; }

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

        item.AddDomainEvent(new BookshelfItemCreatedEvent(item.Id, title));
        return item;
    }

    /// <summary>
    /// Updates book details like pageCount, notes, imageUrl
    /// </summary>
    public void UpdateDetails(int? pageCount = null, string? notes = null, string? imageUrl = null, bool? isRead = null, DateTime? readDate = null)
    {
        PageCount = pageCount;
        Notes = notes;
        ImageUrl = imageUrl;
        
        if (isRead.HasValue)
        {
            IsRead = isRead.Value;
            ReadDate = isRead.Value ? (readDate ?? DateTime.UtcNow) : null;
        }
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

        AddDomainEvent(new BookshelfItemUpdatedEvent(Id, title));
    }

    public void MarkAsRead(DateTime? readDate = null)
    {
        if (IsRead)
            throw new InvalidOperationException("Book is already marked as read");

        IsRead = true;
        ReadDate = readDate ?? DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        if (!IsRead)
            throw new InvalidOperationException("Book is not marked as read");

        IsRead = false;
        ReadDate = null;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("BookshelfItem is already deleted");

        AddDomainEvent(new BookshelfItemDeletedEvent(Id, Title));
    }
}