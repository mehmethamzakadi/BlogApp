using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create
{
    public class CreatePostCommand : IRequest<IResult>
    {
        public string Title { get; set; }
        public string Body { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int CategoriId { get; set; }

        public class CreatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<CreatePostCommand, IResult>
        {
            public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var post = new Post
                    {
                        CategoryId = request.CategoriId,
                        Title = request.Title,
                        Body = request.Body,
                        Summary = request.Summary,
                        Thumbnail = request.Thumbnail,
                        IsPublished = false
                    };
                    await postRepository.AddAsync(post);

                    return new SuccessResult("Post bilgsi başarıyla eklendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Post bilgsi eklerken hata oluştu!");
                }
            }
        }
    }
}
