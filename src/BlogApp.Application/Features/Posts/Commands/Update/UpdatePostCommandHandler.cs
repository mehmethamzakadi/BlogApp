using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdatePostCommand, IResult>
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

        entity.Update(
            request.Title,
            request.Body,
            request.Summary,
            request.CategoryId,
            request.Thumbnail
        );

        // IsPublished durumunu güncelle
        if (request.IsPublished && !entity.IsPublished)
        {
            entity.Publish();
        }
        else if (!request.IsPublished && entity.IsPublished)
        {
            entity.Unpublish();
        }

        await postRepository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla güncellendi.");
    }
}
