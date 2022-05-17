using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using BlogApp.Domain.Entities;
using BlogApp.Application.DTOs.Common;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<BaseResult<CreateCategoryCommand>>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, BaseResult<CreateCategoryCommand>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<CreateCategoryCommand>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                if (!await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Name.ToUpper() == request.Name.ToUpper()))
                {
                    var result = await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = request.Name });
                    await _unitOfWork.SaveAsync();

                    return BaseResult<CreateCategoryCommand>.Success(_mapper.Map<CreateCategoryCommand>(result));
                }

                return BaseResult<CreateCategoryCommand>.Failure("Bu kategori adına ait kayıt zaten bulunmaktadır.");
            }
        }
    }
}
