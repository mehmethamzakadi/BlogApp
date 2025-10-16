
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<UpdatePostCommand, IResult>
{
    public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var entity = await postRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return new ErrorResult("Post bilgisi bulunamadı!");
        }

        entity.Title = request.Title;
        entity.Body = request.Body;
        entity.Summary = request.Summary;
        entity.Thumbnail = request.Thumbnail;
        entity.IsPublished = request.IsPublished;
        entity.CategoryId = request.CategoryId;

        await postRepository.UpdateAsync(entity);

        return new SuccessResult("Post bilgisi başarıyla güncellendi.");
    }
}
