namespace BlogApp.Application.Features.Posts.Queries.GetById
{
    public class GetByIdPostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }
}
