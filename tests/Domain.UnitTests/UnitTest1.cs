
using BlogApp.Domain.Common.Requests;
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

public class PaginationRequestTests
{
    [Test]
    public void PaginatedRequest_Should_Default_To_FirstPage()
    {
        var request = new PaginatedRequest();

        Assert.Multiple(() =>
        {
            Assert.That(request.PageIndex, Is.EqualTo(PaginatedRequest.DefaultPageIndex));
            Assert.That(request.PageSize, Is.EqualTo(PaginatedRequest.DefaultPageSize));
        });
    }

    [Test]
    public void PaginatedRequest_Should_Clamp_PageSize()
    {
        var request = new PaginatedRequest
        {
            PageIndex = -1,
            PageSize = PaginatedRequest.MaxPageSize + 10
        };

        Assert.Multiple(() =>
        {
            Assert.That(request.PageIndex, Is.EqualTo(PaginatedRequest.DefaultPageIndex));
            Assert.That(request.PageSize, Is.EqualTo(PaginatedRequest.MaxPageSize));
        });
    }

    [Test]
    public void DataGridRequest_DefaultCtor_Should_Create_PaginatedRequest()
    {
        var request = new DataGridRequest();

        Assert.That(request.PaginatedRequest, Is.Not.Null);
    }
}
