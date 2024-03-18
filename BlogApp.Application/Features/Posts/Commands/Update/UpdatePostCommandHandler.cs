using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update;

public sealed class UpdatePostCommandHandler(IPostRepository postRepository) : IRequestHandler<UpdatePostCommand, IResult>
{
    public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Post? entity = await postRepository.GetAsync(x => x.Id == request.Id);
            if (entity is null)
                return new ErrorResult("Post bilgisi bulunamadı!");

            entity.Title = request.Title;
            entity.Body = request.Body;
            entity.Summary = request.Summary;
            entity.Thumbnail = request.Thumbnail;
            entity.IsPublished = request.IsPublished;
            entity.CategoryId = request.CategoriId;

            await postRepository.UpdateAsync(entity);

            return new SuccessResult("Post bilgisi başarıyla güncellendi.");
        }
        catch (Exception)
        {
            return new ErrorResult("Post bilgisi güncellenirken hata oluştu.");
        }
    }
}
