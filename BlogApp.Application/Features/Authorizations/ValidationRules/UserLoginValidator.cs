using BlogApp.Application.Features.Authorizations.Queries;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.Authorizations.ValidationRules
{
    public class UserLoginValidator : AbstractValidator<UserLoginQuery>
    {
        public UserLoginValidator()
        {
            RuleFor(u => u.Email).EmailAddress(EmailValidationMode.AspNetCoreCompatible).WithMessage("Email adresi geçersiz!");
        }
    }
}
