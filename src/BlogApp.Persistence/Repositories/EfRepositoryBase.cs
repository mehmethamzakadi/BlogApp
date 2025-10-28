using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Dynamic;
using BlogApp.Domain.Common.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace BlogApp.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TContext>(TContext context) : IAsyncRepository<TEntity>, IRepository<TEntity>
where TEntity : BaseEntity
where TContext : DbContext
{
    protected readonly TContext Context = context;

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        await Context.AddAsync(entity);
        // SaveChanges kaldırıldı - UnitOfWork sorumlu
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        MarkCreatedDates(entities);
        await Context.AddRangeAsync(entities);
        // SaveChanges kaldırıldı
        return entities;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include: null, withDeleted, enableTracking);
        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        queryable = ApplyOrdering(queryable, orderBy);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entity, permanent);
        // SaveChanges kaldırıldı
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        await SetEntityAsDeletedAsync(entities, permanent);
        // SaveChanges kaldırıldı
        return entities;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        queryable = ApplyOrdering(queryable, orderBy);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<Paginate<TEntity>> GetPaginatedListByDynamicAsync(DynamicQuery? dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
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

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        MarkUpdatedDate(entity);
        Context.Update(entity);
        // SaveChanges kaldırıldı
        return Task.FromResult(entity);
    }

    Task<ICollection<TEntity>> IAsyncRepository<TEntity>.UpdateRange(ICollection<TEntity> entities)
    {
        MarkUpdatedDates(entities);
        Context.UpdateRange(entities);
        // SaveChanges kaldırıldı
        return Task.FromResult(entities);
    }

    protected async Task SetEntityAsDeletedAsync(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            await setEntityAsSoftDeletedAsync(entity);
        }
        else
        {
            Context.Remove(entity);
        }
    }

    protected void SetEntityAsDeleted(TEntity entity, bool permanent)
    {
        if (!permanent)
        {
            CheckHasEntityHaveOneToOneRelation(entity);
            setEntityAsSoftDeleted(entity);
        }
        else
        {
            Context.Remove(entity);
        }
    }

    protected void CheckHasEntityHaveOneToOneRelation(TEntity entity)
    {
        bool hasEntityHaveOneToOneRelation =
            Context
                .Entry(entity)
                .Metadata.GetForeignKeys()
                .All(
                    x =>
                        x.DependentToPrincipal?.IsCollection == true
                        || x.PrincipalToDependent?.IsCollection == true
                        || x.DependentToPrincipal?.ForeignKey.DeclaringEntityType.ClrType == entity.GetType()
                ) == false;
        if (hasEntityHaveOneToOneRelation)
            throw new InvalidOperationException(
                "Entity has one-to-one relationship. Soft Delete causes problems if you try to create entry again by same foreign key."
            );
    }

    private async Task setEntityAsSoftDeletedAsync(BaseEntity entity)
    {
        if (entity.DeletedDate.HasValue)
            return;
        entity.DeletedDate = DateTime.UtcNow;
        entity.IsDeleted = true;

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();

        // PERFORMANS İYİLEŞTİRMESİ: Tüm ilişkileri eager load et (N+1 sorgu problemi çözümü)
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            if (navigation.IsCollection)
            {
                await Context.Entry(entity).Collection(navigation.PropertyInfo.Name).LoadAsync();
            }
            else
            {
                await Context.Entry(entity).Reference(navigation.PropertyInfo.Name).LoadAsync();
            }
        }

        // İlişkili entity'leri işle
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            
            if (navigation.IsCollection)
            {
                if (navValue != null)
                {
                    foreach (BaseEntity navValueItem in (IEnumerable)navValue)
                        await setEntityAsSoftDeletedAsync(navValueItem);
                }
            }
            else
            {
                if (navValue != null)
                {
                    await setEntityAsSoftDeletedAsync((BaseEntity)navValue);
                }
            }
        }

        Context.Update(entity);
    }

    private void setEntityAsSoftDeleted(BaseEntity entity)
    {
        if (entity.DeletedDate.HasValue)
            return;
        entity.DeletedDate = DateTime.UtcNow;
        entity.IsDeleted = true;

        var navigations = Context
            .Entry(entity)
            .Metadata.GetNavigations()
            .Where(x => x is { IsOnDependent: false, ForeignKey.DeleteBehavior: DeleteBehavior.ClientCascade or DeleteBehavior.Cascade })
            .ToList();

        // PERFORMANS İYİLEŞTİRMESİ: Tüm ilişkileri eager load et (N+1 sorgu problemi çözümü)
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            if (navigation.IsCollection)
            {
                Context.Entry(entity).Collection(navigation.PropertyInfo.Name).Load();
            }
            else
            {
                Context.Entry(entity).Reference(navigation.PropertyInfo.Name).Load();
            }
        }

        // İlişkili entity'leri işle
        foreach (INavigation? navigation in navigations)
        {
            if (navigation.TargetEntityType.IsOwned())
                continue;
            if (navigation.PropertyInfo == null)
                continue;

            object? navValue = navigation.PropertyInfo.GetValue(entity);
            
            if (navigation.IsCollection)
            {
                if (navValue != null)
                {
                    foreach (BaseEntity navValueItem in (IEnumerable)navValue)
                        setEntityAsSoftDeleted(navValueItem);
                }
            }
            else
            {
                if (navValue != null)
                {
                    setEntityAsSoftDeleted((BaseEntity)navValue);
                }
            }
        }

        Context.Update(entity);
    }

    protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navigationPropertyType)
    {
        Type queryProviderType = query.Provider.GetType();
        MethodInfo createQueryMethod =
            queryProviderType
                .GetMethods()
                .First(m => m is { Name: nameof(query.Provider.CreateQuery), IsGenericMethod: true })
                ?.MakeGenericMethod(navigationPropertyType)
            ?? throw new InvalidOperationException("CreateQuery<TElement> method is not found in IQueryProvider.");
        var queryProviderQuery =
            (IQueryable<object>)createQueryMethod.Invoke(query.Provider, parameters: new object[] { query.Expression })!;
        return queryProviderQuery.Where(x => !((BaseEntity)x).DeletedDate.HasValue);
    }

    protected async Task SetEntityAsDeletedAsync(IEnumerable<TEntity> entities, bool permanent)
    {
        foreach (TEntity entity in entities)
            await SetEntityAsDeletedAsync(entity, permanent);
    }

    public TEntity? Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        return queryable.FirstOrDefault(predicate);
    }

    public Paginate<TEntity> GetList(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include, withDeleted, enableTracking);
        queryable = ApplyOrdering(queryable, orderBy);
        return queryable.ToPaginate(index, size);
    }

    public Paginate<TEntity> GetListByDynamic(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = BuildQueryable(
            predicate,
            include,
            withDeleted,
            enableTracking,
            queryModifier: q => q.ToDynamic(dynamic)
        );
        return queryable.ToPaginate(index, size);
    }

    public bool Any(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true)
    {
        IQueryable<TEntity> queryable = BuildQueryable(predicate, include: null, withDeleted, enableTracking);
        return queryable.Any();
    }

    public TEntity Add(TEntity entity)
    {
        MarkCreatedDate(entity);
        Context.Add(entity);
        // SaveChanges kaldırıldı
        return entity;
    }

    public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
    {
        MarkCreatedDates(entities);
        Context.AddRange(entities);
        // SaveChanges kaldırıldı
        return entities;
    }

    public TEntity Update(TEntity entity)
    {
        MarkUpdatedDate(entity);
        Context.Update(entity);
        // SaveChanges kaldırıldı
        return entity;
    }

    public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
    {
        MarkUpdatedDates(entities);
        Context.UpdateRange(entities);
        // SaveChanges kaldırıldı
        return entities;
    }

    public TEntity Delete(TEntity entity, bool permanent = false)
    {
        SetEntityAsDeleted(entity, permanent);
        // SaveChanges kaldırıldı
        return entity;
    }

    public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
    {
        foreach (TEntity entity in entities)
        {
            SetEntityAsDeleted(entity, permanent);
        }
        // SaveChanges kaldırıldı
        return entities;
    }

    private static void MarkCreatedDate(TEntity entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
    }

    private static void MarkCreatedDates(IEnumerable<TEntity> entities)
    {
        DateTime now = DateTime.UtcNow;
        foreach (TEntity entity in entities)
            entity.CreatedDate = now;
    }

    private static void MarkUpdatedDate(TEntity entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
    }

    private static void MarkUpdatedDates(IEnumerable<TEntity> entities)
    {
        DateTime now = DateTime.UtcNow;
        foreach (TEntity entity in entities)
            entity.UpdatedDate = now;
    }

    private IQueryable<TEntity> BuildQueryable(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
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
