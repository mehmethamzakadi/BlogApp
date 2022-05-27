using AutoMapper;
using BlogApp.Application.DTOs.Categories;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetByIdCategoryQuery : IRequest<IDataResult<CategoryResponseDto>>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, IDataResult<CategoryResponseDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<CategoryResponseDto>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                var categoryDto = _mapper.Map<CategoryResponseDto>(category);
                return new SuccessDataResult<CategoryResponseDto>(categoryDto);
            }
        }
    }
}
