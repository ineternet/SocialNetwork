using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SocialSite.Services;

/// <summary>
/// Helper class containing extension methods for database set queries, used by services in this namespace.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Include a navigation property in a query, retrieve the first object and return the value of that property for that object.
    /// </summary>
    /// <param name="query">The query to apply this operation to.</param>
    /// <param name="include">A navigation property path expression. This will be compiled to resolve the property value.</param>
    /// <returns>The value of the navigation property after retrieval, or <see langword="default"/> if there was no first object in the original query.</returns>
    public static TResult? NavigateFirst<T, TResult>(
        this IQueryable<T> query,
        Expression<Func<T, TResult>> include)
    where T : class
        => query.Include(include).FirstOrDefault() is T first
        ? include.Compile()(first)
        : default;

    /// <inheritdoc cref="IncludeAll{T}(IQueryable{T}, IEnumerable{Expression{Func{T, object?}}})"/>
    public static IQueryable<T> IncludeAll<T>(
        this IQueryable<T> query,
        params Expression<Func<T, object?>>[] properties)
    where T : class
        => query.IncludeAll(propertyEnumeration: properties);

    /// <summary>
    /// Include multiple navigation properties in a single query without breaking the builder chain.
    /// </summary>
    /// <param name="query">The query to apply this operation to.</param>
    /// <param name="propertyEnumeration">The navigation property path expressions to include.</param>
    /// <returns>The same <see cref="IQueryable"/> that was the subject of the operation.</returns>
    public static IQueryable<T> IncludeAll<T>(
        this IQueryable<T> query,
        IEnumerable<Expression<Func<T, object?>>> propertyEnumeration)
    where T : class
    {
        foreach (Expression<Func<T, object?>> prop in propertyEnumeration)
            query = query.Include(prop);
        return query;
    }
}
