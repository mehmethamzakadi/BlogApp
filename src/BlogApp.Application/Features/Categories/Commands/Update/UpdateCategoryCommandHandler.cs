
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
        var category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
        {
            return new ErrorResult("Kategori bilgisi bulunamadı!");
        }

        if (category.Name.ToLower() == request.Name.ToLower())
        {
            return new ErrorResult("Kategori bilgisi zaten mevcut!");
        }

        category.Name = request.Name;

        await categoryRepository.UpdateAsync(category);

        // ✅ Raise domain event BEFORE SaveChanges for Outbox Pattern
        var userId = currentUserService.GetCurrentUserId();
        category.AddDomainEvent(new CategoryUpdatedEvent(category.Id, category.Name, userId ?? category.UpdatedById ?? 0));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
    }
}
