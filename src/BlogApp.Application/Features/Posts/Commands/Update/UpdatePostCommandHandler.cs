using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdatePostCommand, IResult>
{
    public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var entity = await postRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return new ErrorResult("Post bilgisi bulunamadı!");
        }

        entity.Title = request.Title;
        entity.Body = request.Body;
        entity.Summary = request.Summary;
        entity.Thumbnail = request.Thumbnail;
        entity.IsPublished = request.IsPublished;
        entity.CategoryId = request.CategoryId;

        await postRepository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ Raise domain event
        var userId = GetCurrentUserId();
        entity.AddDomainEvent(new PostUpdatedEvent(entity.Id, entity.Title, userId ?? entity.UpdatedById ?? 0));

        return new SuccessResult("Post bilgisi başarıyla güncellendi.");
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
