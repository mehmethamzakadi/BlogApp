using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveAsync();

                return Unit.Value;
            }
        }
    }
}
