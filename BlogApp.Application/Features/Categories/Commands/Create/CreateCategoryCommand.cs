using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommand : IRequest<IResult>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult>
        {
            private readonly ICategoryRepository _categoryRepository;

            public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }

            public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    await _categoryRepository.AddAsync(new Category { Name = request.Name });

                    return new SuccessResult("Kategori bilgsi başarıyla eklendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Kategori bilgsi eklerken hata oluştu!");
                }
            }
        }
    }
}
