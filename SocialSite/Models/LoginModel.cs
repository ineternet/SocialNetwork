namespace SocialSite.Models;

/// <summary>
/// Model that represents a request to start the login procedure.
/// </summary>
public class LoginModel
{
    /// <summary>
    /// The user identifier to login.
    /// </summary>
    /// <value>A valid email address or phone number associated with some user.</value>
    [Required(ErrorMessage = "Enter a valid e-mail address or phone number.")]
    public string? LoginIdentifier { get; set; }
}
