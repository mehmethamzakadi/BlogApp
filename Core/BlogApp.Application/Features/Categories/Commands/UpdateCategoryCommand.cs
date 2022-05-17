using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<BaseResult<UpdateCategoryCommand>>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, BaseResult<UpdateCategoryCommand>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<UpdateCategoryCommand>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                if (category == null)
                    return BaseResult<UpdateCategoryCommand>.Failure("Kategori bilgisi bulunamadı.");

                category.Name = request.Name;
                _unitOfWork.CategoryRepository.Update(category);
                await _unitOfWork.SaveAsync();

                return BaseResult<UpdateCategoryCommand>.Success(_mapper.Map<UpdateCategoryCommand>(category));
            }
        }
    }
}
