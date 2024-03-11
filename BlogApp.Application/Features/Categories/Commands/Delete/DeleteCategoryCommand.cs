using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand, IResult>
        {
            public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                Category? category = await categoryRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);
                if (category is null)
                    return new ErrorResult("Kategori bilgisi bulunamadı!");

                await categoryRepository.DeleteAsync(category);

                return new SuccessResult("Kategori bilgisi başarıyla silindi.");
            }
        }
    }
}
