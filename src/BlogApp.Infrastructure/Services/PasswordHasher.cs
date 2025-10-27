using BlogApp.Application.Abstractions.Identity;
using System.Security.Cryptography;

namespace BlogApp.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    public string HashPassword(string password)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA512);

        var hash = Convert.ToBase64String(algorithm.GetBytes(HashSize));
        var salt = Convert.ToBase64String(algorithm.Salt);

        return $"{Iterations}.{salt}.{hash}";
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);

        using var algorithm = new Rfc2898DeriveBytes(
            providedPassword,
            salt,
            iterations,
            HashAlgorithmName.SHA512);

        var hashToCompare = algorithm.GetBytes(HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }

    public string GeneratePasswordResetToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
