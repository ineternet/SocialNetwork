global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
using static System.Reflection.BindingFlags;
using System.Numerics;
using System.Reflection;
using System.Linq.Expressions;

namespace SocialModel.Entity;

/*
 * The classes in this namespace represent a entity relationship model.
 * They are used to map and generate data sources. EF does this for databases.
 * Do not implement new functionality or properties in these classes unless it
 * should be saved in the data store. Instead create extension classes for them.
 */

/// <summary>
/// A generic type representing an entity with its primary key consisting of a single column.
/// </summary>
/// <typeparam name="TKey">The type of the primary key column.</typeparam>
public interface IIdKey<TKey>
    where TKey : IEqualityOperators<TKey, TKey, bool>
{
    /// <summary>
    /// Property that mirrors the primary key column of this entity.
    /// </summary>
    /// <value>The value of the primary key for this object.</value>
    public TKey Id { get; }

    /// <summary>
    /// A method that has to return the string name of the underlying key property.
    /// This is used in building type-indifferent query expressions dynamically.
    /// </summary>
    /// <typeparam name="TIdKey">The type of the IIdKey implementation this is being called on.</typeparam>
    /// <returns>The string name of the real key property.</returns>
    public static abstract string IdProperty<TIdKey>();

    /// <inheritdoc/>
    public bool Equals(object? obj)
        => obj is IIdKey<TKey> { Id: TKey id }
            && id == this.Id;

    /// <inheritdoc/>
    public int GetHashCode()
        => HashCode.Combine(this.Id);
}

/// <summary>
/// An IIdKey entity that can be ordered.
/// </summary>
/// <typeparam name="TKey">A key that implements equality operators and <c>IComparable</c>.</typeparam>
public interface IOrderedIdKey<TKey> : IIdKey<TKey>
    where TKey : IEqualityOperators<TKey, TKey, bool>, IComparable
{
}

/// <summary>
/// An abstract implementation of an IIdKey entity with an <see cref="int">int</see> key column.
/// Classes that derive from this must declare exactly one <see cref="int">int</see> <see cref="IdKeyAttribute">IdKeyAttribute</see> property.
/// </summary>
public abstract class IdKeyEntity : IOrderedIdKey<int>, IEquatable<IdKeyEntity>
{
    /// <summary>
    /// Mirrors the actual key by use of the custom attribute <see cref="IdKeyAttribute">IdKeyAttribute</see>
    /// </summary>
    /// <value>The real <see cref="int">int</see> value of this object's key.</value>
    public int Id
    {
        get => this.GetType()
                .GetProperties(Public | Instance)
                .Where(x => x.GetCustomAttribute<IdKeyAttribute>() is not null)
                .FirstOrDefault()?
                .GetValue(this) as int? ?? default;
    }
    /// <summary>
    /// Gets the underlying key property's string name by means of the <see cref="IdKeyAttribute">IdKeyAttribute</see> attribute.
    /// </summary>
    /// <typeparam name="TIdKey">The type of the concrete <see cref="IdKeyEntity">IdKeyEntity</see> implementation.</typeparam>
    /// <returns>String name of the supplied type's underlying key property.</returns>
    public static string IdProperty<TIdKey>()
        => typeof(TIdKey)
            .GetProperties(Public | Instance)
            .Where(x => x.GetCustomAttribute<IdKeyAttribute>() is not null)
            .FirstOrDefault()?.Name ?? throw new InvalidOperationException("Call to IdProperty requires IdKeyAttribute");

    /// <inheritdoc/>
    public bool Equals(IdKeyEntity? other) => this == other;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => base.Equals(obj);
    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(this.Id);

    /// <inheritdoc/>
    public static bool operator ==(IdKeyEntity? left, IdKeyEntity? right) => left?.Id == right?.Id;
    /// <inheritdoc/>
    public static bool operator !=(IdKeyEntity? left, IdKeyEntity? right) => left?.Id != right?.Id;

    
}

/// <summary>
/// Declares the <see cref="int">int</see> property to be used as the key column in an entity type.
/// Extending <see cref="IdKeyEntity">IdKeyEntity</see> depends on this.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class IdKeyAttribute : Attribute { }

/// <summary>
/// Describes an EF entity that provides a default navigation expression for avoiding repeated code.
/// </summary>
public interface IDefaultNavigation<TEntity>
{
    /// <summary>
    /// A list of navigation property expressions that will be used with EF's Include method.
    /// </summary>
    /// <value>An unchanging enumeration of property expressions for this entity type.</value>
    public static abstract IEnumerable<Expression<Func<TEntity, object?>>> DefaultNavigation { get; }
}
