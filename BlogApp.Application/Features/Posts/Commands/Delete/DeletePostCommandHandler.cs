using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete;

public sealed class DeletePostCommandHandler(IPostRepository postRepository) : IRequestHandler<DeletePostCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        Post? post = await postRepository.GetAsync(x => x.Id == request.Id);
        if (post is null)
            return Result<string>.FailureResult("Post bilgisi bulunamadı!");

        await postRepository.DeleteAsync(post);

        return Result<string>.SuccessResult("Post bilgisi başarıyla silindi.");
    }
}
