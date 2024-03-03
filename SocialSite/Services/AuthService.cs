using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace SocialSite.Services;

/// <summary>
/// Service that handles user authentication and session management.<br/>
/// Depends on <see cref="ILocalStorageService"/> and <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
/// The injected <see cref="DbContext"/> must provide sets for <see cref="Session"/> and <see cref="SocialModel.Entity.User"/> types.<br/>
/// This class assumes control over the <see cref="LocalStorageSavedToken"/> key in the local storage.
/// </summary>
/// <remarks>
/// Instantiates a new object of the AuthService class.<br/>
/// This class is compatible with dependency injection. This constructor shouldn't be called manually.
/// </remarks>
public sealed class AuthService(ILocalStorageService localStorage, IDbContextFactory<DbContext> dbContext)
{

    /// <summary>
    /// Gets the currently authenticated user, or <see langword="null"/> if authentication is currently incomplete.
    /// </summary>
    /// <value>A user entity for the authenticated user.</value>
    public User? User { get; private set; }

    /// <summary>
    /// Asynchronously retrieves the currently authenticated user, or attempts to authenticate them if there is none.
    /// </summary>
    /// <returns>The authenticated user if possible, or <see langword="null"/> if authentication failed.</returns>
    public async Task<User?> UserAsync()
    {
        if (this.User is null)
            _ = await this.AuthenticateAsync();
        return this.User;
    }

    /// <summary>
    /// Asynchronously logs out a user and wipes the saved token off the current browser's local storage.
    /// <br/>Must be called in OnAfterRenderAsync.
    /// </summary>
    /// <param name="redirect">Optional dependency. If supplied, will force navigation to the login page upon successful logout.</param>
    public async Task ClearAuthAsync(NavigationManager? redirect = null)
    {
        await localStorage.RemoveItemAsync(LocalStorageSavedToken);
        this.User = null;
        redirect?.NavigateTo("/", true);
    }

    /// <summary>
    /// Asynchronously attempts to authenticate the current session by means of the cached token.
    /// <br/>Must be called in OnAfterRenderAsync.
    /// </summary>

    /// <returns>true if authentication was skipped or successful, false if authentication failed.</returns>
    public async Task<bool> AuthenticateAsync(NavigationManager? redirect = null)
    {
        if (this.User is not null)
            return true;

        string? tokenValue;
        try
        {
            tokenValue = await localStorage.GetItemAsStringAsync(LocalStorageSavedToken);
        }
        catch (JSException)
        {
            tokenValue = null;
        }

        await using DbContext dbCtx = await dbContext.CreateDbContextAsync();

        if (Guid.TryParse(tokenValue, out Guid token)
            && (this.User = dbCtx.Set<Session>()
                .Where(x => x.Validated && x.ImpersonateToken == token)
                .IncludeAll(
                    x => x.ProcessOwner.Followers,
                    x => x.ProcessOwner.Following,
                    x => x.ProcessOwner.LikedPosts
                )
                .NavigateFirst(x => x.ProcessOwner))
                is not null)
        {
            return true;
        }
        redirect?.NavigateTo("/", true);
        return false;
    }

    /// <summary>
    /// Asynchronously starts a login procedure for a given user identifier (email address or phone number).
    /// </summary>
    /// <param name="identifier">The user identifier to check against. Must be a valid email address or phone number.</param>
    /// <returns>The generated request ID. This is no indication whether the attempt was successful or not.</returns>
    public async Task<Guid> StartLoginAsync(string identifier)
    {
        await using DbContext dbCtx = await dbContext.CreateDbContextAsync();

        User? user = dbCtx.Set<User>().Where(x =>
            x.EmailAddress == identifier
            || x.PhoneNumber == identifier)
            .FirstOrDefault();

        var requestId = Guid.NewGuid();
        string? completelink = null;

        if (user is not null)
        {
            var secret = Guid.NewGuid().ToString();
            var secretentropy = Guid.NewGuid();
            var secretstore = secret + secretentropy;
            var secrethash = SHA256.HashData(Encoding.UTF8.GetBytes(secretstore));
            completelink = $"/auth/complete/{requestId}/{secret}";

            Console.WriteLine(completelink);
            //await MailService.SendAsync(new(user.EmailAddress, user.Username), completelink);

            Session session = new()
            {
                ProcessOwner = user,
                RequestId = requestId,
                RequestSecretEntropy = secretentropy,
                RequestSecretHash = secrethash,
                ImpersonateToken = Guid.NewGuid(),
                Validated = false
            };
            await dbCtx.Set<Session>().AddAsync(session);
            await dbCtx.SaveChangesAsync();
        }

        return requestId;
    }

    /// <summary>
    /// Asynchronously completes the final login procedure step. The information needed for this step is enveloped within the mail that the first step sent.
    /// </summary>
    /// <param name="requestid">The request ID, generated in the first step.</param>
    /// <param name="secret">The request secret, sent in the mail message.</param>
    /// <param name="redirect">Optional dependency. If supplied, will force navigation to the home page upon successful login.</param>
    /// <returns>Whether the completion attempt was successful or not.</returns>
    public async Task<bool> CompleteLoginAsync(Guid requestid, string secret, NavigationManager? redirect = null)
    {
        await using DbContext dbCtx = await dbContext.CreateDbContextAsync();

        Session? result = dbCtx.Set<Session>().Where(x =>
            x.RequestId == requestid
            && !x.Validated)
            .FirstOrDefault();

        if (result is not Session sess
            || !sess.RequestSecretHash.SequenceEqual(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(
                        secret + sess.RequestSecretEntropy))))
        {
            return false;
        }

        sess.Validated = true;
        await dbCtx.SaveChangesAsync();

        await localStorage.SetItemAsStringAsync(LocalStorageSavedToken, sess.ImpersonateToken.ToString());
        redirect?.NavigateTo("/me", true);
        return true;
    }
}
