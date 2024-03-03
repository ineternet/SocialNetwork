using System.Collections.ObjectModel;
using System.Linq.Expressions;
using SocialModel.Validation;

namespace SocialModel.Entity;

/// <summary>
/// Models a social media post that is either a root post or a reply.
/// One of <see cref="ContentLocation"/> or <see cref="Text"/> must be not null.
/// </summary>
public class Post : IdKeyEntity, IDefaultNavigation<Post>
{
    /// <summary>
    /// The entire primary key, <see cref="int">int</see> identity column.
    /// </summary>
    [IdKey]
    public int PostId { get; init; }

    /// <summary>
    /// A link to content that should be embedded into this post.
    /// </summary>
    /// <value>A Uri describing some internet location, or <see langword="null"/> if this post has no media content.</value>
    [Url, StringLength(2048)]
    public Uri? ContentLocation { get; set; }

    /// <summary>
    /// The content type to expect from the embedded media.
    /// This is set once when the post is created. If this doesn't match the actual media type, the media has changed in the meantime and shouldn't be displayed.
    /// </summary>
    /// <value>A MIME type string describing the media content, or <see langword="null"/> if <see cref="ContentLocation"/> is not set.</value>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types">MDN Documentation on common MIME types</seealso>
    [OneOf, StringLength(100)]
    public string? ContentType { get; set; }

    /// <summary>
    /// The textual content of the post.
    /// </summary>
    /// <value>The post's raw string content, or <see langword="null"/> if this post has no text to display.</value>
    [OneOf, StringLength(5000)]
    public string? Text { get; set; }

    /// <summary>
    /// This post's corresponding parent post. Whether this is set describes if this post is a root post or a reply.
    /// </summary>
    /// <value>
    /// If this is a reply, the other <see cref="Post"/> that is a parent to this post.
    /// <see langword="null"/> if this this a root post.
    /// </value>
    public Post? Parent { get; set; }

    /// <summary>
    /// A collection of all replies to this post.
    /// This is a EF collection navigation property.
    /// </summary>
    /// <value>A list containing all posts that are a reply to this post. Must be loaded by EF.</value>
    public ICollection<Post> Replies { get; set; } = [];

    /// <summary>
    /// A collection of all <see cref="User">Users</see> that have liked this post.
    /// This is a EF collection navigation property.
    /// </summary>
    /// <value>A list containing all users that are currently liking this post. Must be loaded by EF.</value>
    [InverseProperty("LikedPosts")]
    public ICollection<User> Likes { get; set; } = [];
    
    /// <summary>
    /// This post's author.
    /// </summary>
    /// <value>A <see cref="User"/> describing the creator of this post.</value>
    public required User Author { get; set; }

    /// <inheritdoc/>
    public static IEnumerable<Expression<Func<Post, object?>>> DefaultNavigation
        =>
            [
                x => x.Parent,
                x => x.Author,
                x => x.Replies,
                x => x.Likes
            ];
}
