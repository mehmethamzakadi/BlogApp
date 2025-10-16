
using BlogApp.Domain.Entities;

namespace Domain.UnitTests;

public class CommentTests
{
    [Test]
    public void Comment_Should_NotInstantiatePost_ByDefault()
    {
        var comment = new Comment();

        Assert.That(comment.Post, Is.Null);
    }

    [Test]
    public void Comment_Allows_Assigning_Post_Navigation()
    {
        var comment = new Comment();
        var post = new Post { Id = 42, Title = "Test", Body = "Body", Summary = "Summary", Thumbnail = "thumb", CategoryId = 1 };

        comment.Post = post;

        Assert.That(comment.Post, Is.SameAs(post));
        Assert.That(comment.PostId, Is.EqualTo(0));
    }
}
