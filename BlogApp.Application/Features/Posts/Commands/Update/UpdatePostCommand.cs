using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update
{
    public class UpdatePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }
        public virtual List<int> CategoriIds { get; set; }

        public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;
            private readonly IPostCategoryRepository _postCategoryRepository;

            public UpdatePostCommandHandler(IPostRepository postRepository, IPostCategoryRepository postCategoryRepository)
            {
                _postRepository = postRepository;
                _postCategoryRepository = postCategoryRepository;
            }

            public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var exists = await _postRepository.AnyAsync(x => x.Id == request.Id);
                    if (!exists)
                        return new ErrorResult("Post bilgisi bulunamadı!");

                    var entity = await _postRepository.GetAsync(x => x.Id == request.Id);
                    entity.Title = request.Title;
                    entity.Body = request.Body;
                    entity.Summary = request.Summary;
                    entity.Thumbnail = request.Thumbnail;
                    entity.IsPublished = request.IsPublished;

                    await _postRepository.UpdateAsync(entity);

                    //Önce eski postCategories verileri siliniyor
                    var postCategories = await _postCategoryRepository.GetListAsync(x => x.PostId == request.Id);
                    foreach (var postCategory in postCategories.Items)
                    {
                        await _postCategoryRepository.DeleteAsync(postCategory);
                    }

                    //Sonra yeni postCategories verileri ekleniyor.
                    foreach (var categoryId in request.CategoriIds)
                    {
                        await _postCategoryRepository.AddAsync(new PostCategory { CategoryId = categoryId, PostId = request.Id });
                    }

                    return new SuccessResult("Post bilgisi başarıyla güncellendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Post bilgisi güncellenirken hata oluştu.");
                }
            }
        }
    }
}
