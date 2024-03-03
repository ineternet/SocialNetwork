using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SocialModel.Entity;

/// <summary>
/// Models a user that is an actor within the social network.
/// </summary>
public class User : IdKeyEntity, IDefaultNavigation<User>
{
    /// <summary>
    /// The entire primary key, <see cref="int"/> identity column.
    /// </summary>
    [IdKey]
    public int UserId { get; init; }

    /// <summary>
    /// A changeable name the user has chosen for themselves. May be less restrictive on useable characters.
    /// If not set, the user wishes to display their username as their chosen name.
    /// </summary>
    /// <value>The user's chosen display name string, or <see langword="null"/> if the user has unset this.</value>
    [StringLength(50)]
    public string? ChosenName { get; set; }

    /// <summary>
    /// The user's simple name. This should be unique for all users.
    /// </summary>
    /// <value>The user's username as a string.</value>
    [StringLength(40)]
    public required string Username { get; set; }

    /// <summary>
    /// Returns the display name, which is the chosen name, or the username if the chosen name was unset or never set.
    /// This is not mapped into the database.
    /// </summary>
    [NotMapped]
    public string DisplayName => this.ChosenName ?? this.Username;

    /// <summary>
    /// The user's phone number. Knowing this allows a person to start a login request on their behalf.
    /// </summary>
    /// <value>A <see cref="PhoneAttribute"/> validated string describing the user's phone number.</value>
    [Phone, StringLength(20)]
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// The user's email address. Knowing this allows a person to start a login request on their behalf.
    /// Login request completion messages are sent to this address via SMTP.
    /// </summary>
    /// <value>A <see cref="EmailAddressAttribute"/> validated string describing the user's email address.</value>
    [EmailAddress, StringLength(255)]
    public required string EmailAddress { get; set; }

    /// <summary>
    /// Link to a picture to be displayed on this user's profile and posts.
    /// By default, this is cropped and clipped to fit a circle.
    /// If this is not supplied by the user on user creation, the default image has to be used.
    /// </summary>
    /// <value>A Uri describing some internet location with an image.</value>
    /// <seealso cref="SocialSettings.IInstanceConfig.DefaultUserImage">The default image saved in the config.</seealso>
    [Url, StringLength(2048)]
    public required Uri Picture { get; set; }

    /// <summary>
    /// Link to a large rectangular picture to be displayed on this user's profile.
    /// The user can change this at any time. If this is unset, by default a solid fill color will be used.
    /// </summary>
    /// <value>A Uri describing some internet location with an image.</value>
    [Url, StringLength(2048)]
    public Uri? Banner { get; set; }

    /// <summary>
    /// A status message the user has chosen for themselves.
    /// If not set, the user wishes not to display a status. By default this is then either not rendered or replaced with a "No status set" message.
    /// </summary>
    /// <value>The user's chosen status string, or <see langword="null"/> if the user has unset this.</value>
    [StringLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// A collection of all <see cref="Post">Posts</see> this user has liked.
    /// This is a EF collection navigation property.
    /// </summary>
    /// <value>A list containing all posts this user is currently liking. Must be loaded by EF.</value>
    [InverseProperty(nameof(Post.Likes))]
    public ICollection<Post> LikedPosts { get; set; } = [];

    /// <summary>
    /// A collection of all <see cref="User">Users</see> that are following this user object.
    /// This is a EF collection navigation property.
    /// </summary>
    /// <value>A list containing all users currently following the user object. Will never contain the user object itself. Must be loaded by EF.</value>
    public ICollection<User> Followers { get; set; } = [];

    /// <summary>
    /// A collection of all <see cref="User">Users</see> the user object is following.
    /// This is a EF collection navigation property.
    /// </summary>
    /// <value>A list containing all users the user object is currently following. Will never contain the user object itself. Must be loaded by EF.</value>
    public ICollection<User> Following { get; set; } = [];

    /// <inheritdoc/>
    public static IEnumerable<Expression<Func<User, object?>>> DefaultNavigation
        =>
            [
                x => x.Followers,
                x => x.Following,
                x => x.LikedPosts
            ];
}
