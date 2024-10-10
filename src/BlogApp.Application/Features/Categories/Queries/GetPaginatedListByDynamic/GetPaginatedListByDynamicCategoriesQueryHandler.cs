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
        //Paginate<Category> categories = await categoryRepository.GetPaginatedListAsync(
        //index: request.DataGridRequest.PaginatedRequest.PageIndex,
        //size: request.DataGridRequest.PaginatedRequest.PageSize,
        //orderBy: x => x.OrderByDescending(x => x.CreatedDate),
        //cancellationToken: cancellationToken
        //);

        Paginate<Category> categoriesDynamic = await categoryRepository.GetPaginatedListByDynamicAsync(
        dynamic: request.DataGridRequest.DynamicQuery,
        index: request.DataGridRequest.PaginatedRequest.PageIndex,
        size: request.DataGridRequest.PaginatedRequest.PageSize,
        cancellationToken: cancellationToken
        );

        PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>>(categoriesDynamic);
        return response;
    }
}
