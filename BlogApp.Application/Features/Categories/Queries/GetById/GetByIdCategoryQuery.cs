using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetById
{
    public class GetByIdCategoryQuery : IRequest<GetByIdCategoryResponse>
    {
        public int Id { get; set; }

        public class GetCategoryByIdQueryHandler : IRequestHandler<GetByIdCategoryQuery, GetByIdCategoryResponse>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly IMapper _mapper;

            public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
            {
                _categoryRepository = categoryRepository;
                _mapper = mapper;
            }

            public async Task<GetByIdCategoryResponse> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
            {
                Category? category = await _categoryRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);

                GetByIdCategoryResponse response = _mapper.Map<GetByIdCategoryResponse>(category);

                return response;
            }
        }
    }
}
