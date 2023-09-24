using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetList
{
    public class GetListPostQuery : IRequest<IDataResult<IReadOnlyList<GetListPostResponse>>>
    {
        public class GetListPostQueryHandler : IRequestHandler<GetListPostQuery, IDataResult<IReadOnlyList<GetListPostResponse>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public GetListPostQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IDataResult<IReadOnlyList<GetListPostResponse>>> Handle(GetListPostQuery request, CancellationToken cancellationToken)
            {
                var postList = await _unitOfWork.PostRepository.GetAllAsync();
                List<GetListPostResponse> response = _mapper.Map<IReadOnlyList<GetListPostResponse>>(postList).ToList();
                return new SuccessDataResult<IReadOnlyList<GetListPostResponse>>(response);
            }
        }
    }
}
