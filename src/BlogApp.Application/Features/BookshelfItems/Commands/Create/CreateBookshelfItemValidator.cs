using FluentValidation;

namespace BlogApp.Application.Features.BookshelfItems.Commands.Create;

public sealed class CreateBookshelfItemValidator : AbstractValidator<CreateBookshelfItemCommand>
{
    public CreateBookshelfItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Kitap adı boş olmamalıdır!")
            .MaximumLength(200).WithMessage("Kitap adı 200 karakterden uzun olmamalıdır!");

        RuleFor(x => x.Author)
            .MaximumLength(150).WithMessage("Yazar bilgisi 150 karakterden uzun olmamalıdır!")
            .When(x => !string.IsNullOrWhiteSpace(x.Author));

        RuleFor(x => x.Publisher)
            .MaximumLength(150).WithMessage("Yayınevi bilgisi 150 karakterden uzun olmamalıdır!")
            .When(x => !string.IsNullOrWhiteSpace(x.Publisher));

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Sayfa sayısı 0'dan büyük olmalıdır!")
            .LessThanOrEqualTo(20000).WithMessage("Sayfa sayısı 20000'den fazla olmamalıdır!")
            .When(x => x.PageCount.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Not alanı 2000 karakterden fazla olmamalıdır!")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.ReadDate)
            .NotNull().WithMessage("Okunma tarihi, kitap okundu olarak işaretlendiğinde zorunludur.")
            .When(x => x.IsRead);

        RuleFor(x => x.ReadDate)
            .Must(date => date == null || date.Value.Date <= DateTime.UtcNow.Date)
            .WithMessage("Okunma tarihi gelecekte olamaz!");
    }
}
