using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SocialModel.Entity;
using SocialSettings;

namespace SocialModel;

/// <inheritdoc/>
public class SocialContext : DbContext
{

    /// <summary>
    /// A queryable EF DbSet containing all currently loaded <see cref="User">Users</see>.
    /// </summary>
    [NotNull]
    protected DbSet<User>? Users { get; set; }

    /// <summary>
    /// A queryable EF DbSet containing all currently loaded <see cref="Post">Posts</see>.
    /// </summary>
    [NotNull]
    protected DbSet<Post>? Posts { get; set; }

    /// <summary>
    /// A queryable EF DbSet containing all currently loaded <see cref="Session">Sessions</see>.
    /// </summary>
    [NotNull]
    protected DbSet<Session>? Sessions { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            //.UseSqlite(new SqliteConnection("DataSource=file::memory:?cache=shared"));
            .UseSqlServer(DomainIndifferentEnvironment.GetDbConnStr());

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .UseIdentityColumns(100, 10)
            .Entity("UserUser", e => e.ToTable(t =>
                t.HasCheckConstraint("CK_UserUser_CannotFollowSelf", "NOT FollowersUserId = FollowingUserId")
            ))
            .Entity<Post>(e => e.HasOne(e => e.Author).WithMany().OnDelete(DeleteBehavior.NoAction))
            .Entity<Post>(e => e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Post_DeclaresAtLeastMediaOrText", "NOT ([ContentLocation] IS NULL AND [Text] IS NULL)");
                t.HasCheckConstraint("CK_Post_WithMedia_DeclaresContentType", "NOT ([ContentLocation] IS NOT NULL AND [ContentType] IS NULL)");
            }))
            .Entity<User>(e => e.Property(u => u.Picture).HasDefaultValue(Config.Get(c => c.DefaultUserImage)))
            .Entity(DataSeed.Users)
            .Entity(DataSeed.Posts);

    /// <summary>
    /// DbContext factory type that creates DbContexts of the SocialContext type.
    /// </summary>
    public class Factory : IDbContextFactory<DbContext>
    {
        /// <summary>
        /// Create a new DbContext of SocialContext type.
        /// </summary>
        /// <returns>A new DbContext of SocialContext type.</returns>
        public DbContext CreateDbContext() => new SocialContext();
    }
}

internal static partial class DataSeed
{
    internal static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User>> Users = delegate { };
    internal static Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User>> Posts = delegate { };
}
