using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ICacheService cacheService) : IRequestHandler<GetByIdCategoryQuery, IDataResult<GetByIdCategoryResponse>>
{
    public async Task<IDataResult<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Category(request.Id);
        var cacheValue = await cacheService.Get<GetByIdCategoryResponse>(cacheKey);
        if (cacheValue is not null)
            return new SuccessDataResult<GetByIdCategoryResponse>(cacheValue);

        Category? category = await categoryRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return new ErrorDataResult<GetByIdCategoryResponse>("Kategori bilgisi bulunamadı.");

        GetByIdCategoryResponse response = mapper.Map<GetByIdCategoryResponse>(category);

        await cacheService.Add(
            cacheKey,
            response,
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        return new SuccessDataResult<GetByIdCategoryResponse>(response);
    }
}
