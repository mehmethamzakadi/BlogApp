using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetAll;

public sealed class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllListCategoriesQuery, IQueryable>
{
    public Task<IQueryable> Handle(GetAllListCategoriesQuery request, CancellationToken cancellationToken)
    {
        IQueryable query = categoryRepository
            .Query()
            .Where(x => x.IsDeleted == false)
            .Select(x => new
            {
                x.Id,
                x.Name
            });

        return Task.FromResult(query);
    }
}
