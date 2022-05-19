using AutoMapper;
using BlogApp.Application.DTOs.Categories;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryResponseDto>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryResponseDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<CategoryResponseDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                var result = _mapper.Map<IReadOnlyList<CategoryResponseDto>>(categories);
                return result;
            }
        }
    }
}
