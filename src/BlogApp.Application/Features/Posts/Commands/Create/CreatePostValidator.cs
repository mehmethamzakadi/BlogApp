using BlogApp.Domain.Repositories;
using FluentValidation;

namespace BlogApp.Application.Features.Posts.Commands.Create
{
    /// <summary>
    /// Validator for CreatePostCommand with security rules
    /// Note: Body contains HTML content (blog post), so we allow HTML but validate structure
    /// </summary>
    public sealed class CreatePostValidator : AbstractValidator<CreatePostCommand>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CreatePostValidator(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Başlık bilgisi boş olmamalıdır!")
                .MaximumLength(100).WithMessage("Başlık bilgisi 100 karakterden uzun olmamalıdır!")
                .Must(NotContainDangerousScripts).WithMessage("Başlık tehlikeli script içeremez!");

            RuleFor(c => c.Body)
                .NotEmpty().WithMessage("İçerik bilgisi boş olmamalıdır!")
                .MaximumLength(50000).WithMessage("İçerik çok uzun!")
                .Must(NotContainDangerousScripts).WithMessage("İçerik tehlikeli script içeremez!");

            RuleFor(c => c.Thumbnail)
                .NotEmpty().WithMessage("Küçük resim bilgisi boş olmamalıdır!")
                .MaximumLength(500).WithMessage("Küçük resim URL'i çok uzun!");

            RuleFor(c => c.Summary)
               .NotEmpty().WithMessage("Özet bilgisi boş olmamalıdır!")
               .MaximumLength(400).WithMessage("Özet bilgisi 400 karakterden fazla olmamalıdır!")
               .Must(NotContainDangerousScripts).WithMessage("Özet tehlikeli script içeremez!");

            RuleFor(c => c.CategoryId)
                .NotEmpty().WithMessage("Geçerli bir kategori seçilmelidir!")
                .MustAsync(CategoryExists).WithMessage("Geçersiz kategori seçildi!");
        }

        /// <summary>
        /// Prevents XSS attacks by blocking dangerous scripts
        /// Allows safe HTML tags for blog content
        /// </summary>
        private static bool NotContainDangerousScripts(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            string[] dangerousPatterns = {
                "javascript:",
                "onerror=",
                "onclick=",
                "onload=",
                "<script",
                "eval(",
                "expression(",
                "vbscript:",
                "data:text/html"
            };

            return !dangerousPatterns.Any(pattern => value.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
        {
            return await _categoryRepository.AnyAsync(
                x => x.Id == categoryId && !x.IsDeleted,
                cancellationToken: cancellationToken);
        }
    }
}
