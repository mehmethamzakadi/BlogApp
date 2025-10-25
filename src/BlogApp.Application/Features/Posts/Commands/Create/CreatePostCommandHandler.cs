using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreatePostCommand, IResult>
{
    public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ Raise domain event - Event handler will log the activity
        post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, post.CategoryId, userId ?? post.CreatedById));

        return new SuccessResult("Post bilgisi başarıyla eklendi.");
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
