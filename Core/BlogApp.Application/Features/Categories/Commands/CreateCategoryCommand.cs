using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<IResult>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = request.Name });
                    await _unitOfWork.SaveAsync();
                    return new SuccessResult("Kategori bilgsi başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    return new ErrorResult("Kategori bilgsi eklerken hata oluştu!");
                }
            }
        }
    }
}
