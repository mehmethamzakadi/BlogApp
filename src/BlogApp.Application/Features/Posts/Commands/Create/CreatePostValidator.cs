using BlogApp.Domain.Repositories;
using FluentValidation;

namespace BlogApp.Application.Features.Posts.Commands.Create
{
    public sealed class CreatePostValidator : AbstractValidator<CreatePostCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreatePostValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Başlık bilgisi boş olmamalıdır!")
                .MaximumLength(100).WithMessage("Başlık bilgisi 100 karakterden uzun olmamalıdır!");

            RuleFor(c => c.Body)
                .NotEmpty().WithMessage("İçerik bilgisi boş olmamalıdır!");

            RuleFor(c => c.Thumbnail)
                .NotEmpty().WithMessage("Küçük resim bilgisi boş olmamalıdır!");

            RuleFor(c => c.Summary)
               .NotEmpty().WithMessage("Özet bilgisi boş olmamalıdır!")
               .MaximumLength(400).WithMessage("Özet bilgisi 400 karakterden fazla olmamalıdır!");

            RuleFor(c => c.CategoryId)
                .NotEmpty().WithMessage("Geçerli bir kategori seçilmelidir!")
                .MustAsync(CategoryExists).WithMessage("Geçersiz kategori seçildi!");
        }

        private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.AnyAsync(
                x => x.Id == categoryId && !x.IsDeleted,
                cancellationToken: cancellationToken);
        }
    }
}
