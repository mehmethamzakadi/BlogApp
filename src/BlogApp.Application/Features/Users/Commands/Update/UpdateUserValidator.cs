using FluentValidation;
using System.Text.RegularExpressions;

namespace BlogApp.Application.Features.Users.Commands.Update;

/// <summary>
/// Validator for UpdateUserCommand with security rules
/// </summary>
public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UserNameRegex = new(
        @"^[a-zA-Z0-9_-]{3,50}$",
        RegexOptions.Compiled);

    public UpdateUserValidator()
    {
        // Id validation
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");

        // Username validation
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters")
            .Matches(UserNameRegex)
            .WithMessage("Username can only contain letters, numbers, underscore, or hyphen")
            .Must(NotContainWhitespace)
            .WithMessage("Username cannot contain whitespace");

        // Email validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters")
            .Matches(EmailRegex)
            .WithMessage("Invalid email format")
            .EmailAddress()
            .WithMessage("Invalid email address");
    }

    private static bool NotContainWhitespace(string value)
    {
        return !string.IsNullOrEmpty(value) && !value.Any(char.IsWhiteSpace);
    }
}
