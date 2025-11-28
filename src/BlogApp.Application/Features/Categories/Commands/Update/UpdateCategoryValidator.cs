using BlogApp.Application.Common.Security;
using FluentValidation;

namespace BlogApp.Application.Features.Categories.Commands.Update;

/// <summary>
/// Validator for UpdateCategoryCommand with security rules.
/// Uses HtmlSanitizer-based whitelist approach for XSS prevention.
/// </summary>
public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Kategori adı bilgisi boş olmamalıdır!")
            .MinimumLength(5).WithMessage("Kategori adı en az 5 karakter olmalıdır!")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olmalıdır!")
            .MustBePlainText("Kategori adı HTML veya script içeremez!");
    }
}
