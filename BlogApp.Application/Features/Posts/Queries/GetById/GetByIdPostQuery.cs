using AutoMapper;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetById
{
    public class GetByIdPostQuery : IRequest<GetByIdPostResponse>
    {
        public int Id { get; set; }

        public class GetPostByIdQueryHandler : IRequestHandler<GetByIdPostQuery, GetByIdPostResponse>
        {

            private readonly IPostRepository _postRepository;
            private readonly IMapper _mapper;

            public GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper)
            {
                _postRepository = postRepository;
                _mapper = mapper;

            }

            public async Task<GetByIdPostResponse> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
            {
                Post? post = await _postRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
                GetByIdPostResponse response = _mapper.Map<GetByIdPostResponse>(post);

                return response;
            }
        }
    }
}
