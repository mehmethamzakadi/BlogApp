using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.PostEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreatePostCommand, IResult>
{
    public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Kategori geçerliliğini kontrol et
        var categoryExists = await categoryRepository.AnyAsync(
            x => x.Id == request.CategoryId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (!categoryExists)
        {
            return new ErrorResult("Geçersiz kategori seçildi!");
        }

        var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;

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

        // ✅ Outbox Pattern için SaveChanges'dan ÖNCE domain event'i tetikle
        post.AddDomainEvent(new PostCreatedEvent(post.Id, post.Title, post.CategoryId, actorId));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla eklendi.");
    }
}
