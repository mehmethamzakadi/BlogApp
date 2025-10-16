
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed class CreatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<CreatePostCommand, IResult>
{
    public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            CategoryId = request.CategoryId,
            Title = request.Title,
            Body = request.Body,
            Summary = request.Summary,
            Thumbnail = request.Thumbnail,
            IsPublished = request.IsPublished
        };

        await postRepository.AddAsync(post);

        return new SuccessResult("Post bilgisi başarıyla eklendi.");
    }
}
