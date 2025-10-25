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
    ICacheService cacheService) : IRequestHandler<GetByIdCategoryQuery, IDataResult<GetByIdCategoryResponse>>
{
    public async Task<IDataResult<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var cacheValue = await cacheService.Get<GetByIdCategoryResponse>($"category-{request.Id}");
        if (cacheValue is not null)
            return new SuccessDataResult<GetByIdCategoryResponse>(cacheValue);

        Category? category = await categoryRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return new ErrorDataResult<GetByIdCategoryResponse>("Kategori bilgisi bulunamadı.");

        GetByIdCategoryResponse response = mapper.Map<GetByIdCategoryResponse>(category);

        return new SuccessDataResult<GetByIdCategoryResponse>(response);
    }
}
