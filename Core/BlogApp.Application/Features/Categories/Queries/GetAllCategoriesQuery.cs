using AutoMapper;
using BlogApp.Application.DTOs;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetAllCategoriesQuery : IRequest<BaseResult<IReadOnlyList<CategoryDto>>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, BaseResult<IReadOnlyList<CategoryDto>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<IReadOnlyList<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                var result = _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
                return BaseResult<IReadOnlyList<CategoryDto>>.Success(result);
            }
        }
    }
}
