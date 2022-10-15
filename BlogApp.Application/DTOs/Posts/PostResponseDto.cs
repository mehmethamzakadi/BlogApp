using BlogApp.Application.DTOs.Common;

namespace BlogApp.Application.DTOs.Posts
{
    public class PostResponseDto : BaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
    }
}
