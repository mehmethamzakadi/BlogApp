using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed class CreatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<CreatePostCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
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

            return Result<string>.SuccessResult("Post bilgsi başarıyla eklendi.");
        }
        catch (Exception)
        {
            return Result<string>.FailureResult("Post bilgsi eklerken hata oluştu!");
        }
    }
}
