using FluentValidation;
using System.Text.RegularExpressions;

namespace BlogApp.Application.Features.Users.Commands.Create;

/// <summary>
/// Validator for CreateUserCommand with enhanced security rules
/// </summary>
public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UserNameRegex = new(
        @"^[a-zA-Z0-9_-]{3,50}$",
        RegexOptions.Compiled);

    public CreateUserValidator()
    {
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

        // Password validation - Strong password policy
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(128)
            .WithMessage("Password cannot exceed 128 characters")
            .Must(ContainUppercase)
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainLowercase)
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(ContainDigit)
            .WithMessage("Password must contain at least one digit")
            .Must(ContainSpecialCharacter)
            .WithMessage("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?)");
    }

    private static bool NotContainWhitespace(string value)
    {
        return !string.IsNullOrEmpty(value) && !value.Any(char.IsWhiteSpace);
    }

    private static bool ContainUppercase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsUpper);
    }

    private static bool ContainLowercase(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsLower);
    }

    private static bool ContainDigit(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Any(char.IsDigit);
    }

    private static bool ContainSpecialCharacter(string password)
    {
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        return !string.IsNullOrEmpty(password) && password.Any(c => specialChars.Contains(c));
    }
}
