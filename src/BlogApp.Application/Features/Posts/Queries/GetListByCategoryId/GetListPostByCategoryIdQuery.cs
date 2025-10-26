using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetListByCategoryId;

public sealed record GetListPostByCategoryIdQuery(PaginatedRequest PageRequest, Guid CategoryId)
    : IRequest<PaginatedListResponse<GetListPostByCategoryIdResponse>>;
