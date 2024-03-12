using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete
{
    public class DeletePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, IResult>
        {
            public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {
                Post? post = await postRepository.GetAsync(x => x.Id == request.Id);
                if (post is null)
                    return new ErrorResult("Post bilgisi bulunamadı!");

                await postRepository.DeleteAsync(post);

                return new SuccessResult("Post bilgisi başarıyla silindi.");
            }
        }
    }
}
