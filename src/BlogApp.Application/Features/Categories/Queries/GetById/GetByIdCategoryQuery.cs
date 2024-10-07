using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById;

public sealed record GetByIdCategoryQuery(int Id) : IRequest<IDataResult<GetByIdCategoryResponse>>;
