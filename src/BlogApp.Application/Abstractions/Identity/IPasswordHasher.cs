namespace BlogApp.Application.Abstractions.Identity;

public interface IPasswordHasher : Domain.Services.IPasswordHasher
{
    string GeneratePasswordResetToken();
}
