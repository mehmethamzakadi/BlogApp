using BlogApp.Domain.Common;

namespace BlogApp.Domain.Entities;

public sealed class Comment : BaseEntity
{
    private Comment() { }

    public Guid? ParentId { get; private set; }
    public Comment? Parent { get; private set; }
    public Guid PostId { get; private set; }
    public Post? Post { get; private set; }
    public string Content { get; private set; } = default!;
    public string CommentOwnerMail { get; private set; } = default!;
    public bool IsPublished { get; private set; }

    internal static Comment Create(Guid postId, string content, string ownerEmail, Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));
        if (string.IsNullOrWhiteSpace(ownerEmail))
            throw new ArgumentException("Owner email cannot be empty", nameof(ownerEmail));

        return new Comment
        {
            PostId = postId,
            ParentId = parentId,
            Content = content,
            CommentOwnerMail = ownerEmail,
            IsPublished = false
        };
    }

    internal void Publish()
    {
        IsPublished = true;
    }

    internal void Unpublish()
    {
        IsPublished = false;
    }
}
