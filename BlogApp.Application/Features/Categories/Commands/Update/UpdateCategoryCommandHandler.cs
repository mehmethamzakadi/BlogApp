using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<UpdateCategoryCommand, IResult>
{
    public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (category is null)
                return new ErrorResult("Kategori bilgisi bulunamadı!");

            category.Name = request.Name;

            await categoryRepository.UpdateAsync(category);

            return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
        }
        catch (Exception)
        {
            return new ErrorResult("Kategori bilgisi güncellenirken hata oluştu.");
        }
    }
}
