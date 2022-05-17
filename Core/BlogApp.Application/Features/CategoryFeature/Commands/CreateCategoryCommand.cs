using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using BlogApp.Application.DTOs.Params;
using BlogApp.Domain.Entities;
using BlogApp.Application.DTOs.Common;

namespace BlogApp.Application.Features.CategoryFeature.Commands
{
    public class CreateCategoryCommand : IRequest<BaseResult<PmCategory>>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, BaseResult<PmCategory>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<PmCategory>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                if (!await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Name.ToUpper() == request.Name.ToUpper()))
                {
                    var result = await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = request.Name });
                    await _unitOfWork.SaveAsync();

                    return BaseResult<PmCategory>.Success(_mapper.Map<PmCategory>(result));
                }

                return BaseResult<PmCategory>.Failure("Bu kategori adına ait kayıt zaten bulunmaktadır.");
            }
        }
    }
}
