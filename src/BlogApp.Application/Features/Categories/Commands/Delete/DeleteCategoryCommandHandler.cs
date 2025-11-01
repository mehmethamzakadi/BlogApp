using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Delete;

/// <summary>
/// Handler for deleting a category
/// </summary>
public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IPostRepository postRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, enableTracking: true, cancellationToken: cancellationToken);
        if (category is null)
            return new ErrorResult("Kategori bilgisi bulunamadı!");

        // ✅ FIXED: Using PostRepository specific method instead of Query() leak on CategoryRepository
        var hasActivePosts = await postRepository.HasActivePostsInCategoryAsync(request.Id, cancellationToken);

        if (hasActivePosts)
            return new ErrorResult("Bu kategoriye ait aktif postlar bulunmaktadır. Önce postları silmeli veya başka kategoriye taşımalısınız.");

        category.Delete();
        categoryRepository.Delete(category);
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
