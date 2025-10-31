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
            return new ErrorResult("Post bilgisi bulunamadı!");

        post.Delete();
        await postRepository.DeleteAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla silindi.");
    }
}
