using Microsoft.EntityFrameworkCore;

namespace SocialSite.Services;

/// <summary>
/// Service that generates a home page aggregate for a user.<br/>
/// Depends on <see cref="AuthService"/> and <see cref="IDbContextFactory{T}"/>.
/// The injected <paramref name="dbContext"/> must provide sets for <see cref="Post"/> and <see cref="User"/> types.
/// </summary>
/// <remarks>
/// Instantiates a new object of the AggregatorService class.<br/>
/// This class is compatible with dependency injection. This constructor shouldn't be called manually.
/// </remarks>
public sealed class AggregatorService(AuthService authService, IDbContextFactory<DbContext> dbContext)
{

    /// <summary>
    /// Retrieve an asynchronous enumeration of posts this service may consider "interesting" for the authenticated user.
    /// </summary>
    /// <returns></returns>
    public async Task<IAsyncEnumerable<Post>> AggregateAsync()
    {
        User? user = authService.User;
        return user is null
            ? throw new InvalidOperationException("Can't aggregate an unauthenticated user")
            : (await dbContext.CreateDbContextAsync()).Set<Post>().IncludeAll(
            x => x.Author,
            x => x.Likes,
            x => x.Parent,
            x => x.Replies)
            .Where(x =>
                x.Author.Followers.Contains(user)
                || x.Author == user
                ).OrderBy(x => -x.PostId).AsAsyncEnumerable();
    }
}
