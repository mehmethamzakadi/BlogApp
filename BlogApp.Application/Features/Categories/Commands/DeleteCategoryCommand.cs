using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;
            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                    return new ErrorResult("Kategori bilgisi bulunamadı!");

                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveAsync();

                return new SuccessResult("Kategori bilgisi başarıyla silindi.");
            }
        }
    }
}
