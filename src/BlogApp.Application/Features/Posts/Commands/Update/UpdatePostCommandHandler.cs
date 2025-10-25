using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.PostEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<UpdatePostCommand, IResult>
{
    public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var entity = await postRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return new ErrorResult("Post bilgisi bulunamadı!");
        }

        // Kategori değiştiriliyorsa geçerliliğini kontrol et
        if (entity.CategoryId != request.CategoryId)
        {
            var categoryExists = await categoryRepository.AnyAsync(
                x => x.Id == request.CategoryId && !x.IsDeleted, 
                cancellationToken: cancellationToken);
                
            if (!categoryExists)
            {
                return new ErrorResult("Geçersiz kategori seçildi!");
            }
        }

        entity.Title = request.Title;
        entity.Body = request.Body;
        entity.Summary = request.Summary;
        entity.Thumbnail = request.Thumbnail;
        entity.IsPublished = request.IsPublished;
        entity.CategoryId = request.CategoryId;

        await postRepository.UpdateAsync(entity);

        // ✅ Outbox Pattern için SaveChanges'dan ÖNCE domain event'i tetikle
        var userId = currentUserService.GetCurrentUserId();
        entity.AddDomainEvent(new PostUpdatedEvent(entity.Id, entity.Title, userId ?? entity.UpdatedById ?? 0));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla güncellendi.");
    }
}
