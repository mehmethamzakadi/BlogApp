using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<BaseResult<DeleteCategoryCommand>>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, BaseResult<DeleteCategoryCommand>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<DeleteCategoryCommand>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                    return BaseResult<DeleteCategoryCommand>.Failure("Kategori bilgisi bulunamadı.");

                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveAsync();

                return BaseResult<DeleteCategoryCommand>.Success(_mapper.Map<DeleteCategoryCommand>(category));
            }
        }
    }
}
