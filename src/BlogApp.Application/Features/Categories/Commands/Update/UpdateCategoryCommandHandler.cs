using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
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
        var userId = currentUserService.GetCurrentUserId();
        category.AddDomainEvent(new CategoryUpdatedEvent(category.Id, category.Name, userId ?? category.UpdatedById ?? Guid.Empty));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
    }
}
