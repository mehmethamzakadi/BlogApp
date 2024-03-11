using BlogApp.Application.Features.Categories.Queries.GetById;
using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommand : IRequest<IResult>
    {
        public string Name { get; set; }

        public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ICacheService cache) : IRequestHandler<CreateCategoryCommand, IResult>
        {
            public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var category = await categoryRepository.AddAsync(new Category { Name = request.Name });

                    await cache.SetDataAsync($"category-{category.Id}", new GetByIdCategoryResponse { Id = category.Id, Name = category.Name }, DateTime.Now.AddMonths(1));

                    return new SuccessResult("Kategori bilgsi başarıyla eklendi.");
                }
                catch (Exception)
                {
                    return new ErrorResult("Kategori bilgsi eklerken hata oluştu!");
                }
            }
        }
    }
}
