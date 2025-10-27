namespace BlogApp.Domain.Common.Requests;

public class PostListRequest : PaginatedRequest
{
    public Guid? CategoryId { get; set; }
}
