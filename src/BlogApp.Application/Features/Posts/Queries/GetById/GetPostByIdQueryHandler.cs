using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Posts.Queries.GetById;

public sealed class GetPostByIdQueryHandler(IPostRepository postRepository) : IRequestHandler<GetByIdPostQuery, IDataResult<GetByIdPostResponse>>
{
    public async Task<IDataResult<GetByIdPostResponse>> Handle(GetByIdPostQuery request, CancellationToken cancellationToken)
    {
        var response = await postRepository.Query()
            .Where(b => b.Id == request.Id && (request.IncludeUnpublished || b.IsPublished))
            .Include(p => p.Category)
            .AsNoTracking()
            .Select(p => new GetByIdPostResponse(
                p.Id,
                p.Title,
                p.Body,
                p.Summary,
                p.Thumbnail,
                p.IsPublished,
                p.Category.Name,
                p.CategoryId,
                p.CreatedDate
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return new ErrorDataResult<GetByIdPostResponse>("Post bilgisi bulunamadı.");

        return new SuccessDataResult<GetByIdPostResponse>(response);
    }
}
