namespace SocialSite.Models;

/// <summary>
/// Model that represents a request to create a new post.
/// </summary>
public class CreatePostModel
{
    /// <summary>
    /// The new post's media content.
    /// At least either this or <see cref="Text"/> must be set.
    /// </summary>
    /// <value>The media Uri to point to, or null to create a post without media.</value>
    [OneOf]
    [Url]
    public string? ContentLocation { get; set; }

    /// <summary>
    /// The new post's textual content.
    /// At least either this or <see cref="ContentLocation"/> must be set.
    /// </summary>
    /// <value>The post text to display, or null to create a post without text.</value>
    [OneOf]
    public string? Text { get; set; }

    /// <summary>
    /// The parent post, if this post is intended to be a reply.
    /// </summary>
    /// <value>The parent post to make this a reply to, or null if this is a root post.</value>
    public Post? Parent { get; set; }

    /// <summary>
    /// Convert this post creation request into a fully initialized <see cref="Post"/> object, validating it.
    /// </summary>
    /// <param name="author">The <see cref="User"/> to set the new post's author to.</param>
    /// <returns>The newly created post, or <see langword="null"/> if there was a validation error.</returns>
    public async Task<Post?> ToPostAsync(User author)
    {
        _ = Uri.TryCreate(this.ContentLocation, UriKind.Absolute, out Uri? contenturi);
        string? contenttype = null;
        if (contenturi is not null)
        {
            using HttpClient client = new();
            using HttpRequestMessage request = new(HttpMethod.Head, contenturi);
            contenttype = (await new HttpClient().SendAsync(request, HttpCompletionOption.ResponseHeadersRead)).Content.Headers.ContentType?.MediaType;

            if (contenttype is not string ct || !AllowedMedia.Contains(ct))
            {
                return null;
            }
        }

        return new()
        {
            Author = author,
            Text = this.Text,
            ContentLocation = contenturi,
            ContentType = contenttype,
            Parent = this.Parent
        };
    }
}
