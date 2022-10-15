using AutoMapper;
using BlogApp.Application.DTOs.Posts;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries
{
    public class GetAllPostsQuery : IRequest<IDataResult<IReadOnlyList<PostResponseDto>>>
    {
        public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, IDataResult<IReadOnlyList<PostResponseDto>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetAllPostsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<PostResponseDto>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
            {
                var postList = await _unitOfWork.PostRepository.GetAllAsync();
                var postDtoList = postList.Select(post => _mapper.Map<PostResponseDto>(post)).ToList();
                return new SuccessDataResult<IReadOnlyList<PostResponseDto>>(postDtoList);
            }
        }
    }
}
