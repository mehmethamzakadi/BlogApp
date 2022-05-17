using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.CategoryFeature.Commands
{
    public class DeleteCategoryCommand : IRequest<BaseResult<Unit>>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, BaseResult<Unit>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseResult<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                    return BaseResult<Unit>.Failure("Kategori bilgisi bulunamadı.");

                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveAsync();

                return BaseResult<Unit>.Success(await Unit.Task);
            }
        }
    }
}
