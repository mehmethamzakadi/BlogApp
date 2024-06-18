using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (category is null)
                return Result<string>.FailureResult("Kategori bilgisi bulunamadı!");

            category.Name = request.Name;

            await categoryRepository.UpdateAsync(category);

            return Result<string>.SuccessResult("Kategori bilgisi başarıyla güncellendi.");
        }
        catch (Exception)
        {
            return Result<string>.FailureResult("Kategori bilgisi güncellenirken hata oluştu.");
        }
    }
}
