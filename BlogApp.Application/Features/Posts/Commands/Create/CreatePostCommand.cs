using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create
{
    public class CreatePostCommand : IRequest<IResult>, ITransactionalRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public int CategoriId { get; set; }

        public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;

            public CreatePostCommandHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }

            public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var postItem = new Post
                    {
                        CategoryId = request.CategoriId,
                        Title = request.Title,
                        Body = request.Body,
                        Summary = request.Summary,
                        Thumbnail = request.Thumbnail,
                        IsPublished = false
                    };
                    var post = await _postRepository.AddAsync(postItem);

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
