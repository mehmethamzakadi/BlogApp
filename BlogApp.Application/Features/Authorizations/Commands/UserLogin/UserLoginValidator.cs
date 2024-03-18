using FluentValidation;
using FluentValidation.Validators;

namespace BlogApp.Application.Features.Authorizations.Commands.UserLogin;

public sealed class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
        RuleFor(u => u.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible).WithMessage("Email adresi geçersiz!");
    }
}
