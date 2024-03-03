using Microsoft.EntityFrameworkCore;

namespace SocialSite.Services;

/// <summary>
/// An implementation of the <see cref="EntityService{TEntity, TKey}"/> service for posts.
/// Provides additional functionality based around post entities.
/// </summary>
/// <remarks>
/// Instantiates a new object of the PostsService class.<br/>
/// This class is compatible with dependency injection. This constructor shouldn't be called manually.
/// </remarks>
public class PostsService(AuthService authService, IDbContextFactory<DbContext> dbContext) : IntKeyEntityService<Post>(authService, dbContext)
{
    /// <summary>
    /// Asynchronously make a given user like the given post.
    /// </summary>
    /// <param name="post">The post to put the like to.</param>
    /// <param name="user">The user that initiated the like operation.</param>
    public async Task PutLikeAsync(Post post, User user)
        => await this.ChangeManyAsync(post, user,
            static (p, u) => p.Likes.Add(u),
            static (p, u) => !p.Likes.Contains(u));

    /// <summary>
    /// Asynchronously make a given user unlike the given post.
    /// </summary>
    /// <param name="post">The post to remove the like from, if possible.</param>
    /// <param name="user">The user that initiated the unlike operation.</param>
    public async Task DeleteLikeAsync(Post post, User user)
        => await this.ChangeManyAsync(post, user,
            static (p, u) => p.Likes.Remove(u));

    /// <summary>
    /// Asynchronously executes a delete statement to delete a post. Only the post's author should be able to do this.
    /// </summary>
    /// <param name="post">The post to delete.</param>
    public async Task DeletePostAsync(Post post)
        => await (await this.DbContextFactory.CreateDbContextAsync()).Set<Post>().Where(x => x.PostId == post.PostId).ExecuteDeleteAsync();
}
