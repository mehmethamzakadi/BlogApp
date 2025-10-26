using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

/// <summary>
/// Custom User Repository - Identity'den bağımsız kullanıcı yönetimi
/// Microsoft'un IPasswordHasher kullanarak güvenli password hashing sağlar
/// </summary>
public sealed class UserRepository : EfRepositoryBase<User, BlogAppDbContext>, IUserRepository
{
    private readonly BlogAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserRepository(
        BlogAppDbContext context, 
        IPasswordHasher<User> passwordHasher) : base(context)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Paginate<User>> GetUsersAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await _context.Users
            .OrderBy(u => u.Id)
            .ToPaginateAsync(index, size, cancellationToken);
    }

    public User? FindById(Guid id)
    {
        return _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
    }

    public async Task<User?> FindByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
    }

    public async Task<IResult> CreateAsync(User user, string password)
    {
        try
        {
            // Normalize username and email
            user.NormalizedUserName = user.UserName.ToUpperInvariant();
            user.NormalizedEmail = user.Email.ToUpperInvariant();

            // Hash password using Microsoft's PasswordHasher (PBKDF2)
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            // Set initial values
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.EmailConfirmed = false;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            user.LockoutEnabled = true;
            user.AccessFailedCount = 0;

            _context.Users.Add(user);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management

            return new SuccessResult("Kullanıcı başarıyla oluşturuldu.");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate key") == true ||
                ex.InnerException?.Message.Contains("IX_Users_NormalizedEmail") == true)
            {
                return new ErrorResult("Bu e-posta adresi zaten kullanılıyor.");
            }
            if (ex.InnerException?.Message.Contains("IX_Users_NormalizedUserName") == true)
            {
                return new ErrorResult("Bu kullanıcı adı zaten kullanılıyor.");
            }
            return new ErrorResult($"Kullanıcı oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> AddToRoleAsync(User user, string roleName)
    {
        var normalizedRoleName = roleName.ToUpperInvariant();
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

        if (role == null)
        {
            return new ErrorResult($"'{roleName}' rolü bulunamadı.");
        }

        var existingUserRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

        if (existingUserRole != null)
        {
            return new ErrorResult("Kullanıcı zaten bu role sahip.");
        }

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id,
            AssignedDate = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management

        return new SuccessResult("Rol başarıyla atandı.");
    }

    public async Task<IResult> UpdatePasswordAsync(Guid userId, string resetToken, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return new ErrorResult("Kullanıcı bulunamadı.");
        }

        // Verify reset token
        if (user.PasswordResetToken != resetToken)
        {
            return new ErrorResult("Geçersiz şifre sıfırlama token'ı.");
        }

        // Check token expiry
        if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            return new ErrorResult("Şifre sıfırlama token'ının süresi dolmuş.");
        }

        // Hash new password
        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

        // Update security stamp to invalidate existing tokens
        user.SecurityStamp = Guid.NewGuid().ToString();

        // Clear reset token
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        _context.Users.Update(user);
        // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management

        return new SuccessResult("Şifre başarıyla güncellendi.");
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return await Task.FromResult(result == PasswordVerificationResult.Success ||
                                      result == PasswordVerificationResult.SuccessRehashNeeded);
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IResult> AddToRolesAsync(User user, params string[] roles)
    {
        var errors = new List<IdentityError>();

        foreach (var roleName in roles)
        {
            var normalizedRoleName = roleName.ToUpperInvariant();
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                errors.Add(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"'{roleName}' rolü bulunamadı."
                });
                continue;
            }

            // ✅ BEST PRACTICE: Delta Update yaklaşımıyla artık duplicate check'e gerek yok
            // Handler zaten sadece eklenecek rolleri gönderiyor
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedDate = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);
        }

        if (errors.Any())
        {
            var errorMessage = string.Join(", ", errors.Select(e => e.Description));
            return new ErrorResult(errorMessage);
        }

        // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management
        return new SuccessResult("Roller başarıyla atandı.");
    }

    public async Task<IResult> RemoveFromRolesAsync(User user, params string[] roles)
    {
        var errors = new List<IdentityError>();

        foreach (var roleName in roles)
        {
            var normalizedRoleName = roleName.ToUpperInvariant();
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName);

            if (role == null)
            {
                errors.Add(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"'{roleName}' rolü bulunamadı."
                });
                continue;
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
            }
        }

        if (errors.Any())
        {
            var errorMessage = string.Join(", ", errors.Select(e => e.Description));
            return new ErrorResult(errorMessage);
        }

        // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management
        return new SuccessResult("Roller başarıyla kaldırıldı.");
    }

    public new async Task<IResult> UpdateAsync(User user)
    {
        try
        {
            // Update normalized fields
            user.NormalizedUserName = user.UserName.ToUpperInvariant();
            user.NormalizedEmail = user.Email.ToUpperInvariant();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            _context.Users.Update(user);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management

            return new SuccessResult("Kullanıcı başarıyla güncellendi.");
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ErrorResult("Kullanıcı başka bir işlem tarafından güncellenmiş. Lütfen tekrar deneyin.");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate key") == true)
            {
                return new ErrorResult("Bu e-posta veya kullanıcı adı zaten kullanılıyor.");
            }
            return new ErrorResult($"Kullanıcı güncellenirken bir hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> DeleteUserAsync(User user)
    {
        try
        {
            _context.Users.Remove(user);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management

            return new SuccessResult("Kullanıcı başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Kullanıcı silinirken bir hata oluştu: {ex.Message}");
        }
    }
}
