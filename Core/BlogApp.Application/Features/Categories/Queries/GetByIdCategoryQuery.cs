using AutoMapper;
using BlogApp.Application.DTOs.Categories;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetByIdCategoryQuery : IRequest<CategoryResponseDto>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, CategoryResponseDto>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<CategoryResponseDto> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                return _mapper.Map<CategoryResponseDto>(category);
            }
        }
    }
}
