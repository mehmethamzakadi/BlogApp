using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cache,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<CreateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        bool categoryExists = await categoryRepository.AnyAsync(x => x.Name.ToLower() == request.Name.ToLower(), cancellationToken: cancellationToken);
        if (categoryExists)
        {
            return new ErrorResult("Kategori bilgisi zaten mevcut!");
        }

        var category = await categoryRepository.AddAsync(new Category { Name = request.Name });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ Raise domain event
        var userId = GetCurrentUserId();
        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, category.Name, userId ?? category.CreatedById));

        await cache.Add(
            $"category-{category.Id}",
            new GetByIdCategoryResponse(Id: category.Id, Name: category.Name),
            DateTimeOffset.UtcNow.AddMonths(1),
            null);

        return new SuccessResult("Kategori bilgisi başarıyla eklendi.");
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
