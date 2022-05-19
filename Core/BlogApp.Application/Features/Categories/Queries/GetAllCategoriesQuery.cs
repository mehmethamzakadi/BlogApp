using AutoMapper;
using BlogApp.Application.DTOs.Categories;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<IDataResult<IReadOnlyList<CategoryResponseDto>>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IDataResult<IReadOnlyList<CategoryResponseDto>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<CategoryResponseDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categoryList = await _unitOfWork.CategoryRepository.GetAllAsync();
                var categoryDtoList = categoryList.Select(category => _mapper.Map<CategoryResponseDto>(category)).ToList();
                return new SuccessDataResult<IReadOnlyList<CategoryResponseDto>>(categoryDtoList);
            }
        }
    }
}
