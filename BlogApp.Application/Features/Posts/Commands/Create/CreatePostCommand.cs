using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
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
        public virtual List<int> CategoriIds { get; set; }

        public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;
            private readonly IPostCategoryRepository _postCategoryRepository;

            public CreatePostCommandHandler(IPostRepository postRepository, IPostCategoryRepository postCategoryRepository)
            {
                _postRepository = postRepository;
                _postCategoryRepository = postCategoryRepository;
            }

            public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var postItem = new Post { Title = request.Title, Body = request.Body, Summary = request.Summary, Thumbnail = request.Thumbnail, IsPublished = false };
                    var post = await _postRepository.AddAsync(postItem);

                    foreach (int id in request.CategoriIds)
                    {
                        await _postCategoryRepository.AddAsync(new PostCategory { CategoryId = id, PostId = post.Id });
                    }

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
