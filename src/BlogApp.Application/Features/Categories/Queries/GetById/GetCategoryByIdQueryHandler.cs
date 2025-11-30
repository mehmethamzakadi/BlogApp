using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService) : IRequestHandler<GetByIdCategoryQuery, IDataResult<GetByIdCategoryResponse>>
{
    public async Task<IDataResult<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Category(request.Id);
        var cacheValue = await cacheService.Get<GetByIdCategoryResponse>(cacheKey);
        if (cacheValue is not null)
            return new SuccessDataResult<GetByIdCategoryResponse>(cacheValue);

        // ✅ FIXED: Using repository-specific method instead of Query() leak
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            return new ErrorDataResult<GetByIdCategoryResponse>("Kategori bilgisi bulunamadı.");

        var response = new GetByIdCategoryResponse(category.Id, category.Name, category.Description, category.ParentId);

        await cacheService.Add(
            cacheKey,
            response,
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        return new SuccessDataResult<GetByIdCategoryResponse>(response);
    }
}
