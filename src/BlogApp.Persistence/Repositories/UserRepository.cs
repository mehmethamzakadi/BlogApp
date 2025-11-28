using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using BlogApp.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public sealed class UserRepository : EfRepositoryBase<User, BlogAppDbContext>, IUserRepository
{
    public UserRepository(BlogAppDbContext context) : base(context)
    {
    }

    public async Task<Paginate<User>> GetUsersAsync(int index, int size, CancellationToken cancellationToken)
    {
        return await Context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Id)
            .ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        return await Context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await Context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
    }

    public async Task<User?> FindByUserNameAsync(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        return await Context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
    }

    public async Task<List<string>> GetRolesAsync(User user)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            // Removed unnecessary Include(ur => ur.Role) to prevent N+1 problem and extra join
            // Select projection already handles the join efficiently
            .Select(ur => ur.Role.Name)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);
    }
}
