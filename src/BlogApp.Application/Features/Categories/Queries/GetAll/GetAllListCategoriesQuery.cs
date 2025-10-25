using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetAll;

public sealed record GetAllListCategoriesQuery() : IRequest<IQueryable>;
