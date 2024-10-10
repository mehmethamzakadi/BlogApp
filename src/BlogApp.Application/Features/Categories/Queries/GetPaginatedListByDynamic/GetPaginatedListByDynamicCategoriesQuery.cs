using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetPaginatedListByDynamic;

public sealed record GetPaginatedListByDynamicCategoriesQuery(PaginatedRequest PageRequest) : IRequest<PaginatedListResponse<GetPaginatedListByDynamicCategoriesResponse>>;