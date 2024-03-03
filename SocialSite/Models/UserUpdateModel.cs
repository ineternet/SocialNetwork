namespace SocialSite.Models;

/// <summary>
/// Model that represents a request to change a user.
/// </summary>
public class UserUpdateModel
{
    /// <summary>
    /// Provides a new user picture.
    /// </summary>
    /// <value>The picture URI to change to, or null to not change it.</value>
    [Url]
    public string? Picture { get; set; }

    /// <summary>
    /// Provides a new user banner.
    /// </summary>
    /// <value>The banner URI to change to, an empty string to unset it, or null to not change it.</value>
    [Url]
    public string? Banner { get; set; }

    /// <summary>
    /// Provides a new user status.
    /// </summary>
    /// <value>The status string, an empty string to unset it, or null to not change it.</value>
    public string? Bio { get; set; }

    /// <summary>
    /// Provides a new user chosen name.
    /// </summary>
    /// <value>The chosen name, an empty string to unset it, or null to not change it.</value>
    public string? DisplayName { get; set; }
}
