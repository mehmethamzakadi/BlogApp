using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicPostsQuery(DataGridRequest DataGridRequest) : IRequest<PaginatedListResponse<GetPaginatedListByDynamicPostsResponse>>;