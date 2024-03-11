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

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, IResult>
        {
            private readonly ICategoryRepository _categoryRepository;
            private readonly ICacheService _cache;

            public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ICacheService cache)
            {
                _categoryRepository = categoryRepository;
                _cache = cache;
            }

            public async Task<IResult> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var category = await _categoryRepository.AddAsync(new Category { Name = request.Name });

                    await _cache.SetDataAsync($"category-{category.Id}", new GetByIdCategoryResponse { Id = category.Id, Name = category.Name }, DateTime.Now.AddMonths(1));

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
