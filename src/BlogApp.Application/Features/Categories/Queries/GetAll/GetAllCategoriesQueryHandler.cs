using AutoMapper;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetAll;

public sealed class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllListCategoriesQuery, IQueryable>
{
    public async Task<IQueryable> Handle(GetAllListCategoriesQuery request, CancellationToken cancellationToken)
    {
        return categoryRepository
            .Query()
            .Where(x => x.IsDeleted == false)
            .Select(x =>
            new
            {
                x.Id,
                x.Name
            });
    }
}
