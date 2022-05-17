using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.CategoryFeature.Commands
{
    public class UpdateCategoryCommand : IRequest<BaseResult<Unit>>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, BaseResult<Unit>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseResult<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                    return BaseResult<Unit>.Failure("Kategori bilgisi bulunamadı.");

                category.Name = request.Name;

                await _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveAsync();

                return BaseResult<Unit>.Success(await Unit.Task);
            }
        }
    }
}
