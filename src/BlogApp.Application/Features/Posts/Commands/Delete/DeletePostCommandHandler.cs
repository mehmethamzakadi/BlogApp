using BlogApp.Application.Common.Constants;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed class DeletePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeletePostCommand, IResult>
{
    public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetAsync(x => x.Id == request.Id, enableTracking: true);
        if (post is null)
            return new ErrorResult(ResponseMessages.Post.NotFound);

        post.Delete();
        postRepository.Delete(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult(ResponseMessages.Post.Deleted);
    }
}
