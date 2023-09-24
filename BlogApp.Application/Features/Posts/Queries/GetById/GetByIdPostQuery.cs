using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetById
{
    public class GetByIdPostQuery : IRequest<IDataResult<GetByIdPostResponse>>
    {
        public int Id { get; set; }

        public class GetPostByIdQueryHandler : IRequestHandler<GetByIdPostQuery, IDataResult<GetByIdPostResponse>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetPostByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<GetByIdPostResponse>> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);
                var response = _mapper.Map<GetByIdPostResponse>(post);
                return new SuccessDataResult<GetByIdPostResponse>(response);
            }
        }
    }
}
