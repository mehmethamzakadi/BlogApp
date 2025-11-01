using System.Text.RegularExpressions;

namespace BlogApp.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }
    public string NormalizedValue { get; }

    private Email(string value)
    {
        Value = value;
        NormalizedValue = value.ToUpperInvariant();
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Exceptions.DomainValidationException("Email cannot be empty");

        if (!EmailRegex.IsMatch(value))
            throw new Exceptions.DomainValidationException("Invalid email format");

        if (value.Length > 256)
            throw new Exceptions.DomainValidationException("Email cannot exceed 256 characters");

        return new Email(value);
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return NormalizedValue == other.NormalizedValue;
    }

    public override bool Equals(object? obj) => obj is Email email && Equals(email);
    public override int GetHashCode() => NormalizedValue.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
