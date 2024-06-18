using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<UpdatePostCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Post? entity = await postRepository.GetAsync(x => x.Id == request.Id);
            if (entity is null)
                return Result<string>.FailureResult("Post bilgisi bulunamadı!");

            entity.Title = request.Title;
            entity.Body = request.Body;
            entity.Summary = request.Summary;
            entity.Thumbnail = request.Thumbnail;
            entity.IsPublished = request.IsPublished;
            entity.CategoryId = request.CategoriId;

            await postRepository.UpdateAsync(entity);

            return Result<string>.SuccessResult("Post bilgisi başarıyla güncellendi.");
        }
        catch (Exception)
        {
            return Result<string>.FailureResult("Post bilgisi güncellenirken hata oluştu.");
        }
    }
}
