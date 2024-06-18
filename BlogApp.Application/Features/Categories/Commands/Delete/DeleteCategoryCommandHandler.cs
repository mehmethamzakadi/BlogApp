using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (category is null)
            return Result<string>.FailureResult("Kategori bilgisi bulunamadı!");

        await categoryRepository.DeleteAsync(category);

        return Result<string>.SuccessResult("Kategori bilgisi başarıyla silindi.");
    }
}
