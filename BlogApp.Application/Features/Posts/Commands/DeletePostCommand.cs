using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands
{
    public class DeletePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;
            public DeletePostCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);
                if (post == null)
                    return new ErrorResult("Post bilgisi bulunamadı!");
                _unitOfWork.PostRepository.Remove(post);

                //Post Category bilgileri siliniyor.
                var postCategories = _unitOfWork.PostCategoryRepository.GetWhere(x => x.PostId == request.Id).ToList();
                foreach (var postCategory in postCategories)
                {
                    _unitOfWork.PostCategoryRepository.Remove(postCategory);
                }

                await _unitOfWork.SaveChangesAsync();

                return new SuccessResult("Post bilgisi başarıyla silindi.");
            }
        }
    }
}
