using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicUsersQuery(DataGridRequest DataGridRequest) : IRequest<PaginatedListResponse<GetPaginatedListByDynamicUsersResponse>>;