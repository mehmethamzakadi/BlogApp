using BlogApp.Application.Abstractions;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cache,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<CreateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // NormalizedName ile case-insensitive kontrol (database index kullanarak)
        var normalizedName = request.Name.ToUpperInvariant();
        bool categoryExists = await categoryRepository.AnyAsync(
            x => x.NormalizedName == normalizedName, 
            cancellationToken: cancellationToken);
            
        if (categoryExists)
        {
            return new ErrorResult("Bu kategori adı zaten mevcut!");
        }

        var category = await categoryRepository.AddAsync(new Category 
        { 
            Name = request.Name,
            NormalizedName = normalizedName
        });

        // ✅ Outbox Pattern için SaveChanges'dan ÖNCE domain event'i tetikle
        var userId = currentUserService.GetCurrentUserId();
        category.AddDomainEvent(new CategoryCreatedEvent(category.Id, category.Name, userId ?? category.CreatedById));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.Add(
            $"category-{category.Id}",
            new GetByIdCategoryResponse(Id: category.Id, Name: category.Name),
            DateTimeOffset.UtcNow.AddMonths(1),
            null);

        return new SuccessResult("Kategori bilgisi başarıyla eklendi.");
    }
}
