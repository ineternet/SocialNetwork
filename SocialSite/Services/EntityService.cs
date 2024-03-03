using System.Linq.Expressions;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace SocialSite.Services;

/// <summary>
/// Service that provides generic wrappers for common database queries.<br/>
/// Depends on <see cref="AuthService"/> and <see cref="DbContext"/>.
/// The injected <see cref="DbContext"/> must provide a set for the supplied <typeparamref name="TEntity"/> and any additional navigation properties that will be included.
/// </summary>
/// <typeparam name="TEntity">The entity type whose database set operations will attempt to query.</typeparam>
/// <typeparam name="TKey">The type for the primary key column as defined in <see cref="IIdKey{TKey}"/>.</typeparam>
/// <remarks>
/// Base constructor for the EntityService class.
/// </remarks>
public abstract class EntityService<TEntity, TKey>(AuthService authService, IDbContextFactory<DbContext> dbContext)
    where TEntity : class, IIdKey<TKey>
    where TKey : IEqualityOperators<TKey, TKey, bool>, IComparable
{
    private protected IDbContextFactory<DbContext> DbContextFactory { get; init; } = dbContext;
    private protected AuthService Auth { get; init; } = authService;

    /// <summary>
    /// Asynchronously attempts to retrieve an object by its key. If it is tracked by the current context, it will return the tracked object. If it is not found, a database query is made.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <returns>The retrieved entity, or <see langword="null"/> if it was not found in the database.</returns>
    public async Task<TEntity?> FindAsync(TKey key)
        => await (await this.DbContextFactory.CreateDbContextAsync()).Set<TEntity>().FindAsync(key);

    /// <inheritdoc cref="GetByIdAsync(TKey, IEnumerable{Expression{Func{TEntity, object?}}})"/>
    public async Task<TEntity?> GetByIdAsync(TKey key,
        params Expression<Func<TEntity, object?>>[] properties)
            => await this.GetByIdAsync(key, propertyEnumeration: properties);

    /// <summary>
    /// Asynchronously attempts to retrieve the first object found by a key, and includes the specified navigation properties.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <param name="propertyEnumeration">The navigation property expressions to include in the query.</param>
    /// <returns>The retrieved entity, or <see langword="null"/> if it was not found in the database.</returns>
    public async Task<TEntity?> GetByIdAsync(TKey key,
        IEnumerable<Expression<Func<TEntity, object?>>> propertyEnumeration)
    {
        ParameterExpression param = Expression.Parameter(typeof(TEntity), "x");
        return await (await this.DbContextFactory.CreateDbContextAsync()).Set<TEntity>()
            .IncludeAll(propertyEnumeration)
            .FirstOrDefaultAsync(
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Equal(
                        Expression.Property(param, TEntity.IdProperty<TEntity>()),
                        Expression.Constant(key, typeof(TKey))
                        ), param));
    }

    /// <summary>
    /// Performs a destructive, database-mirrored update on an existing, maybe-tracked entity using a second short-lived context.
    /// </summary>
    /// <param name="trackedEntity">The entity to change.</param>
    /// <param name="destructor">The action to perform on the entity. Must be static.</param>
    /// <param name="untrackedCondition">An optional condition to check for on the untracked entity, aborting if it is false.</param>
    public async Task ChangeAsync(TEntity trackedEntity,
        Action<TEntity> destructor,
        Func<TEntity, bool>? untrackedCondition = null)
    {
        await using DbContext ctx = await this.DbContextFactory.CreateDbContextAsync();
        TEntity untrackedEntity = await ctx.Set<TEntity>().Where(x => x == trackedEntity).FirstAsync();

        if (!(untrackedCondition?.Invoke(untrackedEntity) ?? true))
            return;

        destructor(trackedEntity);
        destructor(untrackedEntity);
        _ = await ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Performs a destructive, database-mirrored update on multiple existing, maybe-tracked entities using a second short-lived context.
    /// </summary>
    public async Task ChangeManyAsync<TOther>(
        TEntity trackedEntity, TOther trackedOther,
        Action<TEntity, TOther> destructor,
        Func<TEntity, TOther, bool>? untrackedCondition = null)
    where TOther : class, IIdKey<TKey>
    {
        await using DbContext ctx = await this.DbContextFactory.CreateDbContextAsync();
        TEntity untrackedEntity = await ctx.Set<TEntity>().Where(x => x == trackedEntity).FirstAsync();
        TOther untrackedOther = await ctx.Set<TOther>().Where(x => x == trackedOther).FirstAsync();

        if (!(untrackedCondition?.Invoke(untrackedEntity, untrackedOther) ?? true))
            return;

        destructor(trackedEntity, trackedOther);
        destructor(untrackedEntity, untrackedOther);

        _ = await ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Assuredly inserts a possibly untracked entity into the database.
    /// </summary>
    /// <param name="newEntity">The entity to insert.</param>
    public async Task InsertAsync(TEntity newEntity)
    {
        await using DbContext ctx = await this.DbContextFactory.CreateDbContextAsync();
        _ = await ctx.AddAsync(newEntity);
        _ = await ctx.SaveChangesAsync();
    }

    /// <summary>
    /// Attaches multiple entities into a new context, effectively inserting any not found entities.
    /// </summary>
    public async Task AttachThenInsertAsync(TEntity newEntity, params object?[] attach)
    {
        await using DbContext ctx = await this.DbContextFactory.CreateDbContextAsync();
        foreach (var ent in
            from object? obj in attach.Append(newEntity)
            where obj is not null
            where ctx.Entry(obj).State == EntityState.Detached
            select obj!)
        {
            _ = ctx.Attach(ent);
        }
        _ = await ctx.SaveChangesAsync();
    }
}

/// <summary>
/// An implementation of the <see cref="EntityService{TEntity, TKey}"/> service for entities with <see cref="int"/> keys.
/// </summary>
/// <remarks>
/// Instantiates a new object of the IntKeyEntityService class, providing <see cref="EntityService{TEntity, TKey}"/> functionality for entities with <see cref="int"/> keys.<br/>
/// This class is compatible with dependency injection. This constructor shouldn't be called manually.
/// </remarks>
public class IntKeyEntityService<TEntity>(AuthService authService, IDbContextFactory<DbContext> dbContext)
    : EntityService<TEntity, int>(authService, dbContext)
    where TEntity : class, IIdKey<int>;
