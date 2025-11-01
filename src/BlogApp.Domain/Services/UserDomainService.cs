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

        user.PasswordHash = _passwordHasher.HashPassword(password);
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        user.EmailConfirmed = false;
        user.PhoneNumberConfirmed = false;
        user.TwoFactorEnabled = false;
        user.LockoutEnabled = true;
        user.AccessFailedCount = 0;

        return new SuccessResult("Password set successfully");
    }

    public IResult ResetPassword(User user, string resetToken, string newPassword)
    {
        if (user.PasswordResetToken != resetToken)
            return new ErrorResult("Invalid reset token");

        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            return new ErrorResult("Reset token has expired");

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        return new SuccessResult("Password reset successfully");
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
