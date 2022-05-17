using AutoMapper;
using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Results;
using BlogApp.Application.Interfaces.Persistence;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries
{
    public class GetByIdCategoryQuery : IRequest<BaseResult<RsCategory>>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, BaseResult<RsCategory>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<BaseResult<RsCategory>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                return BaseResult<RsCategory>.Success(_mapper.Map<RsCategory>(category));
            }
        }
    }
}
