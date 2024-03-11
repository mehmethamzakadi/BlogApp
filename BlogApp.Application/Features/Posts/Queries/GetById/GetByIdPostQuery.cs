using AutoMapper;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Queries.GetById
{
    public class GetByIdPostQuery : IRequest<GetByIdPostResponse>
    {
        public int Id { get; set; }

        public class GetPostByIdQueryHandler(IPostRepository postRepository, IMapper mapper) : IRequestHandler<GetByIdPostQuery, GetByIdPostResponse>
        {
            public async Task<GetByIdPostResponse> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
            {
                Post? post = await postRepository.GetAsync(predicate: b => b.Id == request.Id, cancellationToken: cancellationToken);
                GetByIdPostResponse response = mapper.Map<GetByIdPostResponse>(post);

                return response;
            }
        }
    }
}
