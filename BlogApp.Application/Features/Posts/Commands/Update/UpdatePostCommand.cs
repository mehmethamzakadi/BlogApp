using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Update
{
    public class UpdatePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int CategoriId { get; set; }

        public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;

            public UpdatePostCommandHandler(IPostRepository postRepository)
            {
                _postRepository = postRepository;
            }

            public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    Post? entity = await _postRepository.GetAsync(x => x.Id == request.Id);
                    if (entity is null)
                        return new ErrorResult("Post bilgisi bulunamadı!");

                    entity.Title = request.Title;
                    entity.Body = request.Body;
                    entity.Summary = request.Summary;
                    entity.Thumbnail = request.Thumbnail;
                    entity.IsPublished = request.IsPublished;
                    entity.CategoryId = request.CategoriId;

                    await _postRepository.UpdateAsync(entity);

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
