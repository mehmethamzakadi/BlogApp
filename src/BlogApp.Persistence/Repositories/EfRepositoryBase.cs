using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using BlogApp.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace BlogApp.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TContext>(TContext context) : IRepository<TEntity>
where TEntity : BaseEntity
where TContext : DbContext
{
    protected readonly TContext Context = context;

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await Context.AddAsync(entity);
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        await Context.AddRangeAsync(entities);
        return entities;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include: null, withDeleted, enableTracking);
        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null,
        bool withDeleted = false,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        queryable = ApplyOrdering(queryable, orderBy);
        return await queryable.ToListAsync(cancellationToken);
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        SetEntityAsDeleted(entity, permanent);
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        SetEntityAsDeleted(entities, permanent);
        return entities;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, bool withDeleted = false, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        queryable = ApplyOrdering(queryable, orderBy);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetPaginatedListByDynamicAsync(DynamicQuery? dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(
            predicate,
            include,
            withDeleted,
            enableTracking,
            queryModifier: dynamic is not null ? q => q.ToDynamic(dynamic) : null
        );
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public IQueryable<TEntity> Query() => Context.Set<TEntity>();

    public TEntity Update(TEntity entity)
    {
        Context.Update(entity);
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        Context.UpdateRange(entities);
        return entities;
    }

    protected void SetEntityAsDeleted(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            Context.Update(entity);
        }
        else
        {
            Context.Remove(entity);
        }
    }

    protected void SetEntityAsDeleted(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
            SetEntityAsDeleted(entity, permanent);
    }



    private IQueryable<TEntity> BuildQueryable(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? include,
        bool withDeleted,
        bool enableTracking,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> queryable = Query();
        if (queryModifier != null)
            queryable = queryModifier(queryable);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        return queryable;
    }

    private static IQueryable<TEntity> ApplyOrdering(
        IQueryable<TEntity> queryable,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy)
    {
        return orderBy != null ? orderBy(queryable) : queryable;
    }
}
