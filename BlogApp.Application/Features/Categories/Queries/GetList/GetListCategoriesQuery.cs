using AutoMapper;
using BlogApp.Application.Utilities.Requests;
using BlogApp.Application.Utilities.Responses;
using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Queries.GetList
{
    public class GetListCategoriesQuery : IRequest<GetListResponse<GetListCategoryResponse>>
    {
        public PageRequest PageRequest { get; set; } = new();
        public class GetAllCategoriesQueryHandler : IRequestHandler<GetListCategoriesQuery, GetListResponse<GetListCategoryResponse>>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly IMapper _mapper;

            public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
            {
                _categoryRepository = categoryRepository;
                _mapper = mapper;
            }

            public async Task<GetListResponse<GetListCategoryResponse>> Handle(GetListCategoriesQuery request, CancellationToken cancellationToken)
            {
                Paginate<Category> categories = await _categoryRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                cancellationToken: cancellationToken
                );

                GetListResponse<GetListCategoryResponse> response = _mapper.Map<GetListResponse<GetListCategoryResponse>>(categories);
                return response;
            }
        }
    }
}
