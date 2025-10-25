using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.PostEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed class DeletePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<DeletePostCommand, IResult>
{
    public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetAsync(x => x.Id == request.Id);
        if (post is null)
            return new ErrorResult("Post bilgisi bulunamadı!");

        // ✅ Silme işleminden ÖNCE domain event'i tetikle (title bilgisini yakalamak için)
        var userId = currentUserService.GetCurrentUserId();
        post.AddDomainEvent(new PostDeletedEvent(post.Id, post.Title, userId ?? post.CreatedById));

        await postRepository.DeleteAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla silindi.");
    }
}
