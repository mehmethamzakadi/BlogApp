using AutoMapper;
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

        public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMapper _mapper;

            public UpdatePostCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }

            public async Task<IResult> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var exists = await _unitOfWork.PostRepository.ExistsAsync(x => x.Id == request.Id);
                    if (!exists)
                        return new ErrorResult("Post bilgisi bulunamadı!");

                    var entity = await _unitOfWork.PostRepository.GetByIdAsync(request.Id);
                    entity.Title = request.Title;
                    entity.Body = request.Body;
                    entity.Summary = request.Summary;
                    entity.Thumbnail = request.Thumbnail;
                    entity.IsPublished = request.IsPublished;

                    _unitOfWork.PostRepository.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    //Önce eski postCategories verileri siliniyor
                    var postCategories = _unitOfWork.PostCategoryRepository.GetWhere(x => x.PostId == request.Id).ToList();
                    foreach (var postCategory in postCategories)
                    {
                        _unitOfWork.PostCategoryRepository.Remove(postCategory);
                    }
                    await _unitOfWork.SaveChangesAsync();

                    //Sonra yeni postCategories verileri ekleniyor.
                    foreach (var categoryId in request.CategoriIds)
                    {
                        await _unitOfWork.PostCategoryRepository.AddAsync(new PostCategory { CategoryId = categoryId, PostId = request.Id });
                    }
                    await _unitOfWork.SaveChangesAsync();

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
