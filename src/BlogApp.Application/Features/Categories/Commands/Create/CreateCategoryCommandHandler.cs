using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Application.Common.Constants;
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
            return new ErrorResult(ResponseMessages.Category.AlreadyExists);
        }

        // Parent kontrolü - eğer parentId verilmişse, parent'ın var olduğunu kontrol et
        if (request.ParentId.HasValue)
        {
            var parentExists = await categoryRepository.AnyAsync(
                x => x.Id == request.ParentId.Value && !x.IsDeleted,
                cancellationToken: cancellationToken);

            if (!parentExists)
            {
                return new ErrorResult("Üst kategori bulunamadı.");
            }
        }

        var category = Category.Create(request.Name, request.Description, request.ParentId);
        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.Add(
            CacheKeys.Category(category.Id),
            new GetByIdCategoryResponse(Id: category.Id, Name: category.Name, Description: category.Description, ParentId: category.ParentId),
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        await cache.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult(ResponseMessages.Category.Created);
    }
}
