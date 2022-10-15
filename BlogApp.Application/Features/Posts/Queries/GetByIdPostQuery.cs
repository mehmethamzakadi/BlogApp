using AutoMapper;
using BlogApp.Application.DTOs.Posts;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries
{
    public class GetByIdPostQuery : IRequest<IDataResult<PostResponseDto>>
    {
        public int Id { get; set; }

        public class GetPostByIdQueryHandler : IRequestHandler<GetByIdPostQuery, IDataResult<PostResponseDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetPostByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<PostResponseDto>> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);
                var postDto = _mapper.Map<PostResponseDto>(post);
                return new SuccessDataResult<PostResponseDto>(postDto);
            }
        }
    }
}
