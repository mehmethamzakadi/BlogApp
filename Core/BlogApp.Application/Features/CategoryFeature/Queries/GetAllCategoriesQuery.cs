using AutoMapper;
using BlogApp.Application.DTOs.Results;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.CategoryFeature.Queries
{
    public class GetAllCategoriesQuery : IRequest<IReadOnlyList<RsCategory>>
    {
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<RsCategory>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IReadOnlyList<RsCategory>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                var result = _mapper.Map<IReadOnlyList<RsCategory>>(categories);
                return result;
            }
        }
    }
}
