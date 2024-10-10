using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetList;

public sealed record GetListAppUsersQuery(PaginatedRequest PageRequest) : IRequest<PaginatedListResponse<GetListAppUserResponse>>;
