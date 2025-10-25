using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Users.Queries.GetList;

public sealed record GetListUsersQuery(PaginatedRequest PageRequest) : IRequest<PaginatedListResponse<GetListUserResponse>>;
