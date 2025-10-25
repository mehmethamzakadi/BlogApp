using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed class DeletePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<DeletePostCommand, IResult>
{
    public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetAsync(x => x.Id == request.Id);
        if (post is null)
            return new ErrorResult("Post bilgisi bulunamadı!");

        // ✅ Raise domain event BEFORE deletion (to capture title)
        var userId = GetCurrentUserId();
        post.AddDomainEvent(new PostDeletedEvent(post.Id, post.Title, userId ?? post.CreatedById));

        await postRepository.DeleteAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla silindi.");
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
