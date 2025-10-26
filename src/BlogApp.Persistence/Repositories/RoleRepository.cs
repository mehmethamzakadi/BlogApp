using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public sealed class RoleRepository(BlogAppDbContext context) : EfRepositoryBase<Role, BlogAppDbContext>(context), IRoleRepository
{

    public async Task<Paginate<Role>> GetRoles(int index, int size, CancellationToken cancellationToken)
    {
        return await Context.Roles.ToPaginateAsync(index, size, cancellationToken);
    }

    public Role? GetRoleById(Guid id)
    {
        // ✅ Tracking enabled - Entity will be tracked by EF Core
        // This allows domain events to be properly captured and processed
        var result = Context.Roles
            .FirstOrDefault(x => x.Id == id);
        return result;
    }

    public async Task<IResult> CreateRole(Role role)
    {
        try
        {
            await Context.Roles.AddAsync(role);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management
            return new SuccessResult("Rol başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Rol oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> DeleteRole(Role role)
    {
        try
        {
            Context.Roles.Remove(role);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management
            return new SuccessResult("Rol başarıyla silindi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Rol silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> UpdateRole(Role role)
    {
        try
        {
            Context.Roles.Update(role);
            // ✅ REMOVED: SaveChanges - UnitOfWork is responsible for transaction management
            return new SuccessResult("Rol başarıyla güncellendi.");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Rol güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<Role?> FindByNameAsync(string roleName)
    {
        // ✅ Tracking enabled - Entity will be tracked by EF Core
        // ✅ NormalizedName üzerinden case-insensitive karşılaştırma
        var normalizedName = roleName.ToUpperInvariant();
        return await Context.Roles
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedName);
    }

    public bool AnyRole(string name)
    {
        // ✅ NormalizedName üzerinden case-insensitive karşılaştırma
        var normalizedName = name.ToUpperInvariant();
        var result = Context.Roles
            .Any(x => x.NormalizedName == normalizedName);

        return result;
    }
}
