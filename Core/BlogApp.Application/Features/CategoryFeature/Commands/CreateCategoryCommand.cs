using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using BlogApp.Application.DTOs.Params;
using BlogApp.Domain.Entities;

namespace BlogApp.Application.Features.CategoryFeature.Commands
{
    public class CreateCategoryCommand : IRequest<PmCategory>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, PmCategory>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<PmCategory> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                var result = await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = request.Name });
                await _unitOfWork.SaveAsync();

                return _mapper.Map<PmCategory>(result);
            }
        }
    }
}
