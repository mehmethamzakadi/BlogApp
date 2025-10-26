using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Delete;

public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteCategoryCommand, IResult>
{
    public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return new ErrorResult("Kategori bilgisi bulunamadı!");

        // ✅ Silme işleminden ÖNCE domain event'i tetikle
    var actorId = currentUserService.GetCurrentUserId() ?? SystemUsers.SystemUserId;
    category.AddDomainEvent(new CategoryDeletedEvent(category.Id, category.Name, actorId));

        await categoryRepository.DeleteAsync(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Kategori bilgisi başarıyla silindi.");
    }
}
