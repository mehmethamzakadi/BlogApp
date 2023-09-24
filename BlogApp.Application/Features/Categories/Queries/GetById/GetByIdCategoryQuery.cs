using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById
{
    public class GetByIdCategoryQuery : IRequest<IDataResult<GetByIdCategoryResponse>>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, IDataResult<GetByIdCategoryResponse>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                var categoryDto = _mapper.Map<GetByIdCategoryResponse>(category);
                return new SuccessDataResult<GetByIdCategoryResponse>(categoryDto);
            }
        }
    }
}
