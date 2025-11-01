using BlogApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace BlogApp.Infrastructure.Services;

public sealed class AspNetCorePasswordHasher : Application.Abstractions.Identity.IPasswordHasher
{
    private readonly IPasswordHasher<User> _aspNetHasher;

    public AspNetCorePasswordHasher(IPasswordHasher<User> aspNetHasher)
    {
        _aspNetHasher = aspNetHasher;
    }

    public string HashPassword(string password)
    {
        return _aspNetHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _aspNetHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }

    public string GeneratePasswordResetToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
