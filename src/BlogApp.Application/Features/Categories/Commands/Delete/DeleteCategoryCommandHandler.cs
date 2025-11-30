using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Application.Common.Constants;
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
            return new ErrorResult(ResponseMessages.Category.NotFound);

        // ✅ FIXED: Using PostRepository specific method instead of Query() leak on CategoryRepository
        var hasActivePosts = await postRepository.HasActivePostsInCategoryAsync(request.Id, cancellationToken);

        if (hasActivePosts)
            return new ErrorResult(ResponseMessages.Category.HasActivePosts);

        // Alt kategori kontrolü - eğer alt kategoriler varsa silinemez
        var hasChildren = await categoryRepository.HasChildrenAsync(request.Id, cancellationToken);
        if (hasChildren)
            return new ErrorResult("Bu kategorinin alt kategorileri bulunmaktadır. Önce alt kategorileri silmeniz gerekmektedir.");

        category.Delete();
        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.Remove(CacheKeys.Category(category.Id));

        await cacheService.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult(ResponseMessages.Category.Deleted);
    }
}
