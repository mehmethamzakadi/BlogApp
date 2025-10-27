using BlogApp.Application.Abstractions;
using BlogApp.Application.Common.Caching;
using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using System;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(
            predicate: x => x.Id == request.Id,
            cancellationToken: cancellationToken);

        if (category is null)
        {
            return new ErrorResult("Kategori bilgisi bulunamadı!");
        }

        // Başka bir kategoride aynı isim var mı kontrol et (mevcut kategori hariç)
        var normalizedName = request.Name.ToUpperInvariant();
        bool nameExists = await categoryRepository.AnyAsync(
            x => x.NormalizedName == normalizedName && x.Id != request.Id,
            cancellationToken: cancellationToken);

        if (nameExists)
        {
            return new ErrorResult("Bu kategori adı zaten kullanılıyor!");
        }

        category.Name = request.Name;
        category.NormalizedName = normalizedName;

        await categoryRepository.UpdateAsync(category);

        // ✅ Outbox Pattern için SaveChanges'dan ÖNCE domain event'i tetikle
        var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
        category.AddDomainEvent(new CategoryUpdatedEvent(category.Id, category.Name, actorId));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.Add(
            CacheKeys.Category(category.Id),
            new GetByIdCategoryResponse(category.Id, category.Name),
            DateTimeOffset.UtcNow.Add(CacheDurations.Category),
            null);

        await cacheService.Add(
            CacheKeys.CategoryGridVersion(),
            Guid.NewGuid().ToString("N"),
            null,
            null);

        return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
    }
}
