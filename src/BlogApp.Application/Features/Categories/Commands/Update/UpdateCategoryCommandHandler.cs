
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Events;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateCategoryCommand, IResult>
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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // ✅ Raise domain event
        var userId = GetCurrentUserId();
        category.AddDomainEvent(new CategoryUpdatedEvent(category.Id, category.Name, userId ?? category.UpdatedById ?? 0));

        return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
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
