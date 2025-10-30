using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Posts.Commands.Create;

public sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreatePostCommand, IResult>
{
    public async Task<IResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Kategori geçerliliğini kontrol et
        var categoryExists = await categoryRepository.AnyAsync(
            x => x.Id == request.CategoryId && !x.IsDeleted,
            cancellationToken: cancellationToken);

        if (!categoryExists)
        {
            return new ErrorResult("Geçersiz kategori seçildi!");
        }

        var post = Post.Create(
            request.Title,
            request.Body,
            request.Summary,
            request.CategoryId,
            request.Thumbnail
        );

        if (request.IsPublished)
        {
            post.Publish();
        }

        await postRepository.AddAsync(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Post bilgisi başarıyla eklendi.");
    }
}
