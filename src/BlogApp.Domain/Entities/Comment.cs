using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

/// <summary>
/// Comment aggregate root
/// ✅ DDD: Separated from Post aggregate for proper boundaries
/// </summary>
public sealed class Comment : AggregateRoot
{
    private Comment() { }

    public Guid? ParentId { get; private set; }
    public Comment? Parent { get; private set; }
    public Guid PostId { get; private set; }
    // ✅ DDD: Removed Post navigation - reference by ID only
    public string Content { get; private set; } = default!;
    public string CommentOwnerMail { get; private set; } = default!;
    public bool IsPublished { get; private set; }

    public static Comment Create(Guid postId, string content, string ownerEmail, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        if (string.IsNullOrWhiteSpace(ownerEmail))
            throw new ArgumentException("Owner email cannot be empty", nameof(ownerEmail));
        if (postId == Guid.Empty)
            throw new ArgumentException("PostId is required", nameof(postId));

        return new Comment
        {
            PostId = postId,
            ParentId = parentId,
            Content = content,
            CommentOwnerMail = ownerEmail,
            IsPublished = false
        };
    }

    public void Publish()
    {
        if (IsPublished)
            throw new InvalidOperationException("Comment is already published");

        IsPublished = true;
    }

    public void Unpublish()
    {
        if (!IsPublished)
            throw new InvalidOperationException("Comment is not published");

        IsPublished = false;
    }

    public void Update(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));

        Content = content;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Comment is already deleted");
    }
}
