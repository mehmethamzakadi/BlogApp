using AutoMapper;
using BlogApp.Application.Interfaces.Persistence;
using BlogApp.Application.Utilities.Results;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommand : IRequest<IResult>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, IResult>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var exists = await _unitOfWork.CategoryRepository.ExistsAsync(x => x.Id == request.Id);
                    if (!exists)
                        return new ErrorResult("Kategori bilgisi bulunamadı!");

                    var entity = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
                    entity.Name = request.Name;

                    _unitOfWork.CategoryRepository.Update(entity);
                    await _unitOfWork.SaveChangesAsync();

                    return new SuccessResult("Kategori bilgisi başarıyla güncellendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Kategori bilgisi güncellenirken hata oluştu.");
                }
            }
        }
    }
}
