using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetAll;

/// <summary>
/// Query to get all active categories
/// </summary>
public sealed record GetAllListCategoriesQuery() : IRequest<List<CategoryListItemDto>>;

/// <summary>
/// DTO for category list item
/// </summary>
public sealed record CategoryListItemDto(Guid Id, string Name, string? Description = null, Guid? ParentId = null);
