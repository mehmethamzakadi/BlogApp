using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                {
                    return new ErrorResult("Kategori bilgisi bulunamadı!");
                }

                category.Name = request.Name;

                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveAsync();

                return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
            }
        }
    }
}
