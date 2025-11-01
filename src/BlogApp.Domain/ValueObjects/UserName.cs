using System.Text.RegularExpressions;

namespace BlogApp.Domain.ValueObjects;

public sealed class UserName : IEquatable<UserName>
{
    private static readonly Regex UserNameRegex = new(
        @"^[a-zA-Z0-9_-]{3,50}$",
        RegexOptions.Compiled);

    public string Value { get; }
    public string NormalizedValue { get; }

    private UserName(string value)
    {
        Value = value;
        NormalizedValue = value.ToUpperInvariant();
    }

    public static UserName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Exceptions.DomainValidationException("Username cannot be empty");

        if (!UserNameRegex.IsMatch(value))
            throw new Exceptions.DomainValidationException("Username must be 3-50 characters and contain only letters, numbers, underscore, or hyphen");

        return new UserName(value);
    }

    public bool Equals(UserName? other)
    {
        if (other is null) return false;
        return NormalizedValue == other.NormalizedValue;
    }

    public override bool Equals(object? obj) => obj is UserName userName && Equals(userName);
    public override int GetHashCode() => NormalizedValue.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(UserName userName) => userName.Value;
}
