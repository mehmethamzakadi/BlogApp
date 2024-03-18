using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetList;

public sealed class GetAllCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper) : IRequestHandler<GetListCategoriesQuery, GetListResponse<GetListCategoryResponse>>
{
    public async Task<GetListResponse<GetListCategoryResponse>> Handle(GetListCategoriesQuery request, CancellationToken cancellationToken)
    {
        Paginate<Category> categories = await categoryRepository.GetListAsync(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        GetListResponse<GetListCategoryResponse> response = mapper.Map<GetListResponse<GetListCategoryResponse>>(categories);
        return response;
    }
}
