using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetList;

public sealed record GetListPostQuery(PaginatedRequest PageRequest) : IRequest<PaginatedListResponse<GetListPostResponse>>;