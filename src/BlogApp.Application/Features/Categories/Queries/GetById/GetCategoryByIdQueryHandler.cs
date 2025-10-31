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

        var response = await categoryRepository.Query()
            .Where(b => b.Id == request.Id)
            .AsNoTracking()
            .Select(c => new GetByIdCategoryResponse(c.Id, c.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return new ErrorDataResult<GetByIdCategoryResponse>("Kategori bilgisi bulunamadı.");

        await cacheService.Add(
            cacheKey,
            response,
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        return new SuccessDataResult<GetByIdCategoryResponse>(response);
    }
}
