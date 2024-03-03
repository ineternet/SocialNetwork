global using static SocialSite.Browser;

namespace SocialSite;

/// <summary>
/// Provides static configuration for web-related data.
/// </summary>
public static class Browser
{
    /// <summary>
    /// The local storage key to write the authentication bearer token to.
    /// </summary>
    public const string LocalStorageSavedToken = "savedToken";

    /// <summary>
    /// A collection of all media types allowed to be embedded into a post.<br/>
    /// When creating a new post, the creation service will first perform a HEAD request to determine the content type of the attached media. If it does not match any in this list, the post will fail.
    /// </summary>
    public static IReadOnlyCollection<string> AllowedMedia =>
    [
        // Video files
        // MIME types starting with the 5 letters "video" will be displayed in a HTML5 <video> tag.
        "video/mp4",
        "video/mpeg",
        "video/ogg",
        "video/webm",
        "video/x-msvideo",
        "video/mp2t",
        "video/3gpp",
        "video/3gpp2",

        // Image files
        // MIME types starting with the 5 letters "image" will be displayed in a HTML5 <picture> tag.
        "image/png",
        "image/avif",
        "image/bmp",
        "image/gif",
        "image/jpeg",
        "image/tiff",
    ];
}
