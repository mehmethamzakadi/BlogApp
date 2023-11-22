using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
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
                    var exists = await _postRepository.AnyAsync(x => x.Id == request.Id);
                    if (!exists)
                        return new ErrorResult("Post bilgisi bulunamadı!");

                    var entity = await _postRepository.GetAsync(x => x.Id == request.Id);
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
