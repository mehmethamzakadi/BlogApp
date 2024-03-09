using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, IResult>
        {
            private readonly ICategoryRepository _categoryRepository;

            public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                _categoryRepository = categoryRepository;
            }

            public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                Category? category = await _categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
                if (category is null)
                    return new ErrorResult("Kategori bilgisi bulunamadı!");

                await _categoryRepository.DeleteAsync(category);

                return new SuccessResult("Kategori bilgisi başarıyla silindi.");
            }
        }
    }
}
