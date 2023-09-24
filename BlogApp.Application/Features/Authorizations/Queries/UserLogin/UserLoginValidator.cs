using FluentValidation;
using FluentValidation.Validators;

namespace BlogApp.Application.Features.Authorizations.Queries.UserLogin
{
    public class UserLoginValidator : AbstractValidator<UserLoginQuery>
    {
        public UserLoginValidator()
        {
            RuleFor(u => u.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible).WithMessage("Email adresi geçersiz!");
        }
    }
}
