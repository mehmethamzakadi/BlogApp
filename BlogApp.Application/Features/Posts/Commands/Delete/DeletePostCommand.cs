using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete
{
    public class DeletePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;

            public DeletePostCommandHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }

            public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {
                Post? post = await _postRepository.GetAsync(x => x.Id == request.Id);
                if (post is null)
                    return new ErrorResult("Post bilgisi bulunamadı!");

                await _postRepository.DeleteAsync(post);

                return new SuccessResult("Post bilgisi başarıyla silindi.");
            }
        }
    }
}
