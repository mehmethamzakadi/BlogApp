using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed class DeletePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeletePostCommand, IResult>
{
    public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetAsync(x => x.Id == request.Id);
        if (post is null)
            return new ErrorResult("Post bilgisi bulunamadı!");

        await postRepository.DeleteAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla silindi.");
    }
}
