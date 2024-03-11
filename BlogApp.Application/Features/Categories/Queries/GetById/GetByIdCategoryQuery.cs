using AutoMapper;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById
{
    public class GetByIdCategoryQuery : IRequest<IDataResult<GetByIdCategoryResponse>>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, IDataResult<GetByIdCategoryResponse>>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly IMapper _mapper;
            private readonly ICacheService _cacheService;

            public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper, ICacheService cacheService)
            {
                _categoryRepository = categoryRepository;
                _mapper = mapper;
                _cacheService = cacheService;
            }

            public async Task<IDataResult<GetByIdCategoryResponse>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                var cacheValue = await _cacheService.GetDataAsync<GetByIdCategoryResponse>($"category-{request.Id}");
                if (cacheValue is not null)
                    return new SuccessDataResult<GetByIdCategoryResponse>(cacheValue);

                Category? category = await _categoryRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
                if (category is null)
                    return new ErrorDataResult<GetByIdCategoryResponse>("Kategori bilgisi bulunamadı.");

                GetByIdCategoryResponse response = _mapper.Map<GetByIdCategoryResponse>(category);

                return new SuccessDataResult<GetByIdCategoryResponse>(response);
            }
        }
    }
}
