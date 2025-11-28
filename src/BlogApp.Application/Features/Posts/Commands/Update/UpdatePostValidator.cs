using BlogApp.Application.Common.Security;
using FluentValidation;

namespace BlogApp.Application.Features.Posts.Commands.Update;

/// <summary>
/// Validator for UpdatePostCommand with security rules.
/// Uses HtmlSanitizer-based whitelist approach for XSS prevention.
/// </summary>
public sealed class UpdatePostValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Başlık bilgisi boş olmamalıdır!")
            .MaximumLength(100).WithMessage("Başlık bilgisi 100 karakterden uzun olmamalıdır!")
            .MustBePlainText("Başlık HTML veya script içeremez!");

        RuleFor(c => c.Body)
            .NotEmpty().WithMessage("İçerik bilgisi boş olmamalıdır!")
            .MaximumLength(50000).WithMessage("İçerik çok uzun!")
            .MustBeSafeHtml("İçerik tehlikeli script içeriyor!");

        RuleFor(c => c.Thumbnail)
            .NotEmpty().WithMessage("Küçük resim bilgisi boş olmamalıdır!")
            .MaximumLength(500).WithMessage("Küçük resim URL'i çok uzun!")
            .MustBeSafeUrl("Küçük resim URL'i geçersiz veya güvensiz!");

        RuleFor(c => c.Summary)
            .NotEmpty().WithMessage("Özet bilgisi boş olmamalıdır!")
            .MaximumLength(400).WithMessage("Özet bilgisi 400 karakterden fazla olmamalıdır!")
            .MustBePlainText("Özet HTML veya script içeremez!");

        RuleFor(c => c.CategoryId)
            .NotEmpty().WithMessage("Geçerli bir kategori seçilmelidir!");
    }
}
