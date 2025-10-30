using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Create;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cache,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, IResult>
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

        var category = Category.Create(request.Name);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.Add(
            CacheKeys.Category(category.Id),
            new GetByIdCategoryResponse(Id: category.Id, Name: category.Name),
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        await cache.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult("Kategori bilgisi başarıyla eklendi.");
    }
}
