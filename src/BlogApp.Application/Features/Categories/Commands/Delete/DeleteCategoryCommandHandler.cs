using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Delete;

public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return new ErrorResult("Kategori bilgisi bulunamadı!");

        var hasActivePosts = await categoryRepository.Query()
            .AnyAsync(c => c.Id == request.Id && c.Posts.Any(p => !p.IsDeleted), cancellationToken);

        if (hasActivePosts)
            return new ErrorResult("Bu kategoriye ait aktif postlar bulunmaktadır. Önce postları silmeli veya başka kategoriye taşımalısınız.");

        category.Delete();
        await categoryRepository.DeleteAsync(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.Remove(CacheKeys.Category(category.Id));

        await cacheService.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult("Kategori bilgisi başarıyla silindi.");
    }
}
