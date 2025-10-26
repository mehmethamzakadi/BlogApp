using BlogApp.Domain.Common.Paging;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Persistence.Repositories;

public sealed class RoleRepository : EfRepositoryBase<Role, BlogAppDbContext>, IRoleRepository
{
    private readonly BlogAppDbContext _dbContext;

    public RoleRepository(BlogAppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Paginate<Role>> GetRoles(int index, int size, CancellationToken cancellationToken)
    {
        return await _dbContext.Roles.ToPaginateAsync(index, size, cancellationToken);
    }

    public Role? GetRoleById(Guid id)
    {
        // ✅ Tracking enabled - Entity will be tracked by EF Core
        // This allows domain events to be properly captured and processed
        var result = _dbContext.Roles
            .FirstOrDefault(x => x.Id == id);
        return result;
    }

    public async Task<IResult> CreateRole(Role role)
    {
        try
        {
            await _dbContext.Roles.AddAsync(role);
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
            _dbContext.Roles.Remove(role);
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
            _dbContext.Roles.Update(role);
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
        return await _dbContext.Roles
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedName);
    }

    public bool AnyRole(string name)
    {
        // ✅ NormalizedName üzerinden case-insensitive karşılaştırma
        var normalizedName = name.ToUpperInvariant();
        var result = _dbContext.Roles
            .Any(x => x.NormalizedName == normalizedName);

        return result;
    }
}
