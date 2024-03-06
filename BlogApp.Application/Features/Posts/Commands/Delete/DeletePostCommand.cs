using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Utilities.Results;
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
                var exists = await _postRepository.AnyAsync(x => x.Id == request.Id);
                if (!exists)
                    return new ErrorResult("Post bilgisi bulunamadı!");

                var post = await _postRepository.GetAsync(x => x.Id == request.Id);
                await _postRepository.DeleteAsync(post);

                return new SuccessResult("Post bilgisi başarıyla silindi.");
            }
        }
    }
}
