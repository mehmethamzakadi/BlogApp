using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.CategoryFeature.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                await _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.SaveAsync();

                return await Unit.Task;
            }
        }
    }
}
