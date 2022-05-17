using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Results;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.CategoryFeature.Queries
{
    public class GetAllCategoriesQuery : IRequest<BaseResult<IReadOnlyList<RsCategory>>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, BaseResult<IReadOnlyList<RsCategory>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<IReadOnlyList<RsCategory>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                var result = _mapper.Map<IReadOnlyList<RsCategory>>(categories);
                return BaseResult<IReadOnlyList<RsCategory>>.Success(result);
            }
        }
    }
}
