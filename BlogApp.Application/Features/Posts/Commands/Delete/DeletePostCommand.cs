using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Posts.Commands.Delete
{
    public class DeletePostCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, IResult>, ITransactionalRequest
        {
            private readonly IPostRepository _postRepository;
            private readonly IPostCategoryRepository _postCategoryRepository;

            public DeletePostCommandHandler(IPostRepository postRepository, IPostCategoryRepository postCategoryRepository)
            {
                _postRepository = postRepository;
                _postCategoryRepository = postCategoryRepository;
            }

            public async Task<IResult> Handle(DeletePostCommand request, CancellationToken cancellationToken)
            {

                var exists = await _postRepository.AnyAsync(x => x.Id == request.Id);
                if (!exists)
                    return new ErrorResult("Post bilgisi bulunamadı!");

                var post = await _postRepository.GetAsync(x => x.Id == request.Id);
                await _postRepository.DeleteAsync(post);

                //Post Category bilgileri siliniyor.
                var postCategories = await _postCategoryRepository.GetListAsync(x => x.PostId == request.Id);
                foreach (var postCategory in postCategories.Items)
                {
                    await _postCategoryRepository.DeleteAsync(postCategory);
                }

                return new SuccessResult("Post bilgisi başarıyla silindi.");
            }
        }
    }
}
