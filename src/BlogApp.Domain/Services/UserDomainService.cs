using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using System.Linq;

namespace BlogApp.Domain.Services;

public sealed class UserDomainService : IUserDomainService
{
    private readonly IPasswordHasher _passwordHasher;

    public UserDomainService(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public IResult SetPassword(User user, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return new ErrorResult("Password cannot be empty");

        // ✅ SECURITY: Using User entity behavior method instead of direct property access
        var hashedPassword = _passwordHasher.HashPassword(password);
        user.ChangePassword(hashedPassword);

        return new SuccessResult("Password set successfully");
    }

    public IResult ResetPassword(User user, string resetToken, string newPassword)
    {
        // ✅ SECURITY FIX: Token validation with timing-safe comparison
        if (string.IsNullOrWhiteSpace(user.PasswordResetToken))
            return new ErrorResult("No reset token found for this user");

        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return new ErrorResult("Reset token has expired");

        // ✅ SECURITY: Constant-time comparison to prevent timing attacks
        if (!ConstantTimeEquals(user.PasswordResetToken, resetToken))
            return new ErrorResult("Invalid reset token");

        // ✅ SECURITY: Strong password validation
        if (string.IsNullOrWhiteSpace(newPassword))
            return new ErrorResult("New password cannot be empty");

        if (newPassword.Length < 8)
            return new ErrorResult("Password must be at least 8 characters long");

        var hashedPassword = _passwordHasher.HashPassword(newPassword);
        user.ChangePassword(hashedPassword);

        return new SuccessResult("Password reset successfully");
    }

    /// <summary>
    /// Constant-time string comparison to prevent timing attacks
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a == null || b == null)
            return a == b;

        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }

    public bool VerifyPassword(User user, string password)
    {
        return _passwordHasher.VerifyPassword(user.PasswordHash, password);
    }

    public IResult AddToRole(User user, Role role)
    {
        var existingRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (existingRole != null)
            return new ErrorResult("User already has this role");

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedDate = DateTime.UtcNow
        };

        user.UserRoles.Add(userRole);
        return new SuccessResult("Role assigned successfully");
    }

    public IResult AddToRoles(User user, IEnumerable<Role> roles)
    {
        foreach (var role in roles)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedDate = DateTime.UtcNow
            };
            user.UserRoles.Add(userRole);
        }

        return new SuccessResult("Roles assigned successfully");
    }

    public IResult RemoveFromRoles(User user, IEnumerable<Role> roles)
    {
        var roleIds = roles.Select(r => r.Id).ToHashSet();
        var userRolesToRemove = user.UserRoles.Where(ur => roleIds.Contains(ur.RoleId)).ToList();

        foreach (var userRole in userRolesToRemove)
        {
            user.UserRoles.Remove(userRole);
        }

        return new SuccessResult("Roles removed successfully");
    }
}
