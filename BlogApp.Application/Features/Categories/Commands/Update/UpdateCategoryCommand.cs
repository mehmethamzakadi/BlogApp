using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult>
        {
            private readonly ICategoryRepository _categoryRepository;

            public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }

            public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var category = await _categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
                    if (category is null)
                        return new ErrorResult("Kategori bilgisi bulunamadı!");

                    category.Name = request.Name;

                    await _categoryRepository.UpdateAsync(category);

                    return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Kategori bilgisi güncellenirken hata oluştu.");
                }
            }
        }
    }
}
