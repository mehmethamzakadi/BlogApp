using AutoMapper;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Queries.GetPaginatedListByDynamic;

public sealed class GetPaginatedListByDynamicBookshelfItemsQueryHandler(
    IBookshelfItemRepository bookshelfItemRepository,
    IMapper mapper) : IRequestHandler<GetPaginatedListByDynamicBookshelfItemsQuery, PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse>>
{
    public async Task<PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse>> Handle(GetPaginatedListByDynamicBookshelfItemsQuery request, CancellationToken cancellationToken)
    {
        Paginate<BookshelfItem> items = await bookshelfItemRepository.GetPaginatedListByDynamicAsync(
            dynamic: request.DataGridRequest.DynamicQuery,
            index: request.DataGridRequest.PaginatedRequest.PageIndex,
            size: request.DataGridRequest.PaginatedRequest.PageSize,
            cancellationToken: cancellationToken);

        PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse> response = mapper.Map<PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse>>(items);
        return response;
    }
}
