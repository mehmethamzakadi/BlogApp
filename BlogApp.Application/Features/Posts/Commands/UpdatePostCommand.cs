using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands
{
    public class UpdatePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
        public string Thumbnail { get; set; }
        public bool IsPublished { get; set; }

        public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdatePostCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);
                if (post == null)
                    return new ErrorResult("Post bilgisi bulunamadı!");

                post.Title = request.Title;
                post.Body = request.Body;
                post.Summary = request.Summary;
                post.Thumbnail = request.Thumbnail;
                post.IsPublished = request.IsPublished;

                _unitOfWork.PostRepository.Update(post);
                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Post bilgisi başarıyla güncellendi.");
            }
        }
    }
}
