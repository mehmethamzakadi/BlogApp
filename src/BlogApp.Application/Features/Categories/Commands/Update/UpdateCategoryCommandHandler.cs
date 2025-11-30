using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Application.Common.Constants;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(
            predicate: x => x.Id == request.Id,
            enableTracking: true,
            cancellationToken: cancellationToken);

        if (category is null)
        {
            return new ErrorResult(ResponseMessages.Category.NotFound);
        }

        // Başka bir kategoride aynı isim var mı kontrol et (mevcut kategori hariç)
        var normalizedName = request.Name.ToUpperInvariant();
        bool nameExists = await categoryRepository.AnyAsync(
            x => x.NormalizedName == normalizedName && x.Id != request.Id,
            cancellationToken: cancellationToken);

        if (nameExists)
        {
            return new ErrorResult(ResponseMessages.Category.AlreadyExists);
        }

        // Parent kontrolü - eğer parentId verilmişse, parent'ın var olduğunu ve döngüsel referans olmadığını kontrol et
        if (request.ParentId.HasValue)
        {
            // Kendi kendisinin parent'ı olamaz (bu kontrol entity'de de var ama burada da kontrol ediyoruz)
            if (request.ParentId.Value == request.Id)
            {
                return new ErrorResult("Kategori kendi üst kategorisi olamaz.");
            }

            var parentExists = await categoryRepository.AnyAsync(
                x => x.Id == request.ParentId.Value && !x.IsDeleted,
                cancellationToken: cancellationToken);

            if (!parentExists)
            {
                return new ErrorResult("Üst kategori bulunamadı.");
            }

            // Döngüsel referans kontrolü: Parent'ın kendisi veya üst kategorileri bu kategoriyi içeriyor mu?
            var parentCategory = await categoryRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parentCategory != null)
            {
                // Parent'ın parent'larını kontrol et (basit döngü kontrolü)
                var currentParentId = parentCategory.ParentId;
                while (currentParentId.HasValue)
                {
                    if (currentParentId.Value == request.Id)
                    {
                        return new ErrorResult("Döngüsel kategori referansı oluşturulamaz.");
                    }
                    var currentParent = await categoryRepository.GetByIdAsync(currentParentId.Value, cancellationToken);
                    if (currentParent == null) break;
                    currentParentId = currentParent.ParentId;
                }
            }
        }

        category.Update(request.Name, request.Description, request.ParentId);
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.Add(
            CacheKeys.Category(category.Id),
            new GetByIdCategoryResponse(category.Id, category.Name, category.Description, category.ParentId),
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        await cacheService.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult(ResponseMessages.Category.Updated);
    }
}
