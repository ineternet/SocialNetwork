using Microsoft.EntityFrameworkCore;
using SocialSite.Models;

namespace SocialSite.Services;

/// <summary>
/// An implementation of the <see cref="EntityService{TEntity, TKey}"/> service for users.
/// Provides additional functionality based around user entities.
/// </summary>
/// <remarks>
/// Instantiates a new object of the UsersService class.<br/>
/// This class is compatible with dependency injection. This constructor shouldn't be called manually.
/// </remarks>
public class UsersService(AuthService authService, IDbContextFactory<DbContext> dbContext) : IntKeyEntityService<User>(authService, dbContext)
{

    /// <summary>
    /// Retrieve a user's post index (all non-replies they have made).
    /// </summary>
    /// <param name="user">The user to query for.</param>
    /// <returns>An asynchronous enumeration of all root posts by given user.</returns>
    public async Task<IAsyncEnumerable<Post>> GetRootIndexAsync(User user)
        => (await this.DbContextFactory.CreateDbContextAsync())
            .Set<Post>()
            .IncludeAll(
                x => x.Author,
                x => x.Likes,
                x => x.Replies
            )
            .Where(p => p.Author == user && p.Parent == null)
            .AsAsyncEnumerable();

    /// <summary>
    /// Retrieve a user's reply index (all replies they have made).
    /// </summary>
    /// <param name="user">The user to query for.</param>
    /// <returns>An asynchronous enumeration of all reply posts by given user.</returns>
    public async Task<IAsyncEnumerable<Post>> GetReplyIndexAsync(User user)
        => (await this.DbContextFactory.CreateDbContextAsync())
            .Set<Post>()
            .IncludeAll(
                x => x.Author,
                x => x.Likes,
                x => x.Replies,
                x => x.Parent
            )
            .Where(p => p.Author == user && p.Parent != null)
            .AsAsyncEnumerable();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public async Task ApplyUpdateAsync(User user, UserUpdateModel update)
    {
        await this.ChangeAsync(user, u =>
        {
            if (update.DisplayName is string dn)
                u.ChosenName = dn.All(char.IsWhiteSpace) || dn.Length <= 0 ? null : dn;
            if (update.Bio is string bio)
                u.Bio = bio.All(char.IsWhiteSpace) || bio.Length <= 0 ? null : bio;
        });
    }
}
