using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetAll;

/// <summary>
/// Handler for getting all active categories
/// </summary>
public sealed class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetAllListCategoriesQuery, List<CategoryListItemDto>>
{
    public async Task<List<CategoryListItemDto>> Handle(GetAllListCategoriesQuery request, CancellationToken cancellationToken)
    {
        // ✅ FIXED: Using repository-specific method instead of Query() leak
        // ✅ FIXED: Returning List<DTO> instead of IQueryable (anti-pattern)
        var categories = await categoryRepository.GetAllActiveAsync(cancellationToken);

        return categories.Select(c => new CategoryListItemDto(c.Id, c.Name, c.Description, c.ParentId)).ToList();
    }
}
