namespace SocialSettings;

/// <summary>
/// A collection of properties that are read from a configuration.
/// No objects of this are created on their own.
/// </summary>
public interface IInstanceConfig
{
    /// <summary>
    /// The default picture to use for new users when they don't supply a custom picture on user creation.
    /// </summary>
    /// <value>An internet resource with an image.</value>
    public Uri DefaultUserImage { get; }

    /// <summary>
    /// The page title that will be written in the HTML <c>title</c> tag.
    /// </summary>
    /// <value>The page title as a string.</value>
    public string PageTitle { get; }
}

internal class DevelopmentInstanceConfig : IInstanceConfig
{
    public Uri DefaultUserImage => new("https://secure.gravatar.com/avatar/66d35120ac4d38d9579dd53d6086213e?d=identicon&s=512");
    public string PageTitle => "My Network";
}

/// <summary>
/// A static class for retrieving instance configuration values.
/// </summary>
public static class Config
{
    private static readonly Lazy<IInstanceConfig> Configuration = new(() => new DevelopmentInstanceConfig());

    /// <summary>
    /// Read some value from the current instance configuration.
    /// </summary>
    /// <param name="expr">An operation to perform on the current <see cref="IInstanceConfig"/>.</param>
    /// <typeparam name="T">The type of the returned value.</typeparam>
    /// <returns>The value that was returned by the <paramref name="expr"/> operation.</returns>
    public static T Get<T>(Func<IInstanceConfig, T> expr) => expr(Configuration.Value);

    /// <summary>
    /// Asynchronously read or calculate some value from the current instance configuration.
    /// </summary>
    /// <param name="expr">An asynchronous operation to perform on the current <see cref="IInstanceConfig"/>.</param>
    /// <typeparam name="T">The type of the returned value.</typeparam>
    /// <returns>The task that describes the <paramref name="expr"/> operation.</returns>
    public static async Task<T> Get<T>(Func<IInstanceConfig, Task<T>> expr) => await expr(Configuration.Value);

    /// <summary>
    /// Perform an action having the current instance configuration.
    /// </summary>
    /// <param name="expr">An action to perform using the current <see cref="IInstanceConfig"/>.</param>
    public static void WithConfig(Action<IInstanceConfig> expr) => expr(Configuration.Value);

    /// <summary>
    /// Asynchronously perform an action having the current instance configuration.
    /// </summary>
    /// <param name="expr">An asynchronous action to perform using the current <see cref="IInstanceConfig"/>.</param>
    public static async Task WithConfig(Func<IInstanceConfig, Task> expr) => await expr(Configuration.Value);
}

/// <summary>
/// Static class for reading configuration from the current machine setup rather than those for the current instance.
/// </summary>
public static class DomainIndifferentEnvironment
{
    /// <summary>
    /// Get the configured database connection string, read from environment variables.
    /// </summary>
    /// <returns>A database connection <see langword="string"/>.</returns>
    public static string GetDbConnStr() => SQL_CONNSTR ?? throw new InvalidOperationException("No database connection string found");
    internal static readonly string? SQL_CONNSTR = Environment.GetEnvironmentVariable("SQL_CONNSTR");
}
