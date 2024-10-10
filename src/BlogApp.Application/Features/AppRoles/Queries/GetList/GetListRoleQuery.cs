using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetList;

public sealed record GetListRoleQuery(PaginatedRequest PageRequest) : IRequest<PaginatedListResponse<GetListAppRoleResponse>>;

