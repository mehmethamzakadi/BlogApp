using AutoMapper;
using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;

public sealed class GetPaginatedListByDynamicCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedListByDynamicCategoriesQuery, PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>>
{
    public async Task<PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>> Handle(GetPaginatedListByDynamicCategoriesQuery request, CancellationToken cancellationToken)
    {
        Paginate<Category> categories = await categoryRepository.GetPaginatedListAsync(
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        orderBy: x => x.OrderByDescending(x => x.CreatedDate),
        cancellationToken: cancellationToken
        );


        var dynamic = new DynamicQuery
        {
            Filter = new Filter
            {
                Field = "",
                Operator = "",
                Value = "",
                Logic = ""
            },
            Sort = [new Sort { Dir = "asc", Field = "Name" },]
        };

        Paginate<Category> categoriesDynamic = await categoryRepository.GetPaginatedListByDynamicAsync(
        dynamic: dynamic,
        index: request.PageRequest.PageIndex,
        size: request.PageRequest.PageSize,
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>>(categoriesDynamic);
        return response;
    }
}
