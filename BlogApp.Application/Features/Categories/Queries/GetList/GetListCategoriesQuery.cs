using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetList;

public sealed record GetListCategoriesQuery(PageRequest PageRequest) : IRequest<GetListResponse<GetListCategoryResponse>>;