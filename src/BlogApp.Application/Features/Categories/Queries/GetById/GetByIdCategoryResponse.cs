namespace BlogApp.Application.Features.Categories.Queries.GetById;

public sealed record GetByIdCategoryResponse(Guid Id, string Name, string? Description = null, Guid? ParentId = null);
