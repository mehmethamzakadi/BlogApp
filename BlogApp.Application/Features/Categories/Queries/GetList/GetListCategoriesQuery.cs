using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetList
{
    public class GetListCategoriesQuery : IRequest<IDataResult<IReadOnlyList<GetListCategoryResponse>>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetListCategoriesQuery, IDataResult<IReadOnlyList<GetListCategoryResponse>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<GetListCategoryResponse>>> Handle(GetListCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categoryList = await _unitOfWork.CategoryRepository.GetAllAsync();
                List<GetListCategoryResponse> response = _mapper.Map<IReadOnlyList<GetListCategoryResponse>>(categoryList).ToList();
                return new SuccessDataResult<IReadOnlyList<GetListCategoryResponse>>(response);
            }
        }
    }
}
