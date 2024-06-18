using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetList;

public sealed record GetListCategoriesQuery(PageRequest PageRequest) : IRequest<Result<GetListResponse<GetListCategoryResponse>>>;