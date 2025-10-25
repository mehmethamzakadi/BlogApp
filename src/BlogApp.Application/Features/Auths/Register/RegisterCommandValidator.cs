using FluentValidation;

namespace BlogApp.Application.Features.Auths.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Kullanıcı adı boş olamaz!")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır!")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir!")
            .Matches("^[a-zA-Z0-9\\-._@]+$").WithMessage("Kullanıcı adı sadece harf, rakam ve -._@ karakterlerini içerebilir!");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş olamaz!")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre boş olamaz!")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır!")
            .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir!")
            .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir!")
            .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir!")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir!")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir!");
    }
}
