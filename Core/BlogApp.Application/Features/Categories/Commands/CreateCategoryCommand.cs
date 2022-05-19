using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;
using BlogApp.Domain.Entities;
using BlogApp.Application.DTOs.Common;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<CreateCategoryCommand>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryCommand>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<CreateCategoryCommand> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {

                var result = await _unitOfWork.CategoryRepository.AddAsync(new Category { Name = request.Name });
                await _unitOfWork.SaveAsync();

                return _mapper.Map<CreateCategoryCommand>(result);
            }
        }
    }
}
