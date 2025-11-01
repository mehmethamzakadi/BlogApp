using FluentValidation;

namespace BlogApp.Application.Features.Categories.Commands.Create
{
    /// <summary>
    /// Validator for CreateCategoryCommand with security rules
    /// </summary>
    public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Kategori adı bilgisi boş olmamalıdır!")
                .MinimumLength(5).WithMessage("Kategori adı en az 5 karakter olmalıdır!")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olmalıdır!")
                .Must(NotContainHtmlTags).WithMessage("Kategori adı HTML/script içeremez!")
                .Must(NotContainDangerousCharacters).WithMessage("Kategori adı tehlikeli karakterler içeremez!");
        }

        /// <summary>
        /// Prevents XSS attacks by blocking HTML tags
        /// </summary>
        private static bool NotContainHtmlTags(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            return !value.Contains('<') && !value.Contains('>');
        }

        /// <summary>
        /// Prevents injection attacks
        /// </summary>
        private static bool NotContainDangerousCharacters(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            string[] dangerousPatterns = { "javascript:", "onerror=", "onclick=", "<script", "eval(", "expression(" };
            return !dangerousPatterns.Any(pattern => value.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}
