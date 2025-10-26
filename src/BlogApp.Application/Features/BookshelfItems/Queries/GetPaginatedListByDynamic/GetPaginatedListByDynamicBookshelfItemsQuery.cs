using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.BookshelfItems.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicBookshelfItemsQuery(DataGridRequest DataGridRequest) : IRequest<PaginatedListResponse<GetPaginatedListByDynamicBookshelfItemsResponse>>;
