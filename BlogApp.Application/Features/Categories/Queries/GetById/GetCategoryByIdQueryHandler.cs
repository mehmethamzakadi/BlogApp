using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ICacheService cacheService) : IRequestHandler<GetByIdCategoryQuery, Result<GetByIdCategoryResponse>>
{
    public async Task<Result<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var cacheValue = await cacheService.Get<GetByIdCategoryResponse>($"category-{request.Id}");
        if (cacheValue is not null)
            return Result<GetByIdCategoryResponse>.SuccessResult(cacheValue);

        Category? category = await categoryRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return Result<GetByIdCategoryResponse>.FailureResult("Kategori bilgisi bulunamadı.");

        GetByIdCategoryResponse response = mapper.Map<GetByIdCategoryResponse>(category);

        return Result<GetByIdCategoryResponse>.SuccessResult(response);
    }
}
