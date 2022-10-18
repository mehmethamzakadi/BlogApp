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
                var exists = await _unitOfWork.CategoryRepository.ExistsAsync(x => x.Id == request.Id);
                if (!exists)
                    return new ErrorResult("Kategori bilgisi bulunamadı!");

                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Kategori bilgisi başarıyla silindi.");
            }
        }
    }
}
