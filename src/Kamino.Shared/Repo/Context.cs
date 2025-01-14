using Kamino.Shared.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kamino.Shared.Repo;

public abstract class Context(DbContextOptions options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Ping> Pings { get; set; }
    public DbSet<Pong> Pongs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Build base models
        modelBuilder.Entity<Follow>().IsIdentifiableEntity();
        modelBuilder.Entity<Like>().IsIdentifiableEntity();
        modelBuilder.Entity<Ping>().IsIdentifiableEntity();
        modelBuilder.Entity<Pong>().IsIdentifiableEntity();
        modelBuilder.Entity<Profile>().IsBasicEntity();
        modelBuilder.Entity<Post>().IsBasicEntity();
        modelBuilder.Entity<Place>().IsBasicEntity();

        // Build required columns
        modelBuilder.Entity<Follow>(entityBuilder =>
        {
            entityBuilder.Property(p => p.ActorUri).IsRequired();
            entityBuilder.Property(p => p.ObjectUri).IsRequired();
        });
        modelBuilder.Entity<Like>(entityBuilder =>
        {
            entityBuilder.Property(p => p.ActorUri).IsRequired();
            entityBuilder.Property(p => p.ObjectUri).IsRequired();
        });
        modelBuilder.Entity<Ping>(entityBuilder =>
        {
            entityBuilder.Property(p => p.ActorUri).IsRequired();
            entityBuilder.Property(p => p.ToUri).IsRequired();
        });
        modelBuilder.Entity<Tag>(entityBuilder =>
        {
            entityBuilder.Property(p => p.NormalizedTitle).IsRequired();
        });

        // Build primary keys
        modelBuilder.Entity<Tag>(entityBuilder =>
        {
            entityBuilder.HasKey(p => p.NormalizedTitle);
        });

        // Build alternate keys
        modelBuilder.Entity<Follow>(entityBuilder =>
        {
            entityBuilder.HasAlternateKey(p => new { p.ActorUri, p.ObjectUri });
        });
        modelBuilder.Entity<Like>(entityBuilder =>
        {
            entityBuilder.HasAlternateKey(p => new { p.ActorUri, p.ObjectUri });
        });

        // Build one-to-many relationships
        modelBuilder
            .Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(p => p.PostsAuthored)
            .IsRequired();
        modelBuilder
            .Entity<Place>()
            .HasOne(p => p.Author)
            .WithMany(p => p.PlacesAuthored)
            .IsRequired();
        modelBuilder
            .Entity<Ping>()
            .HasOne(p => p.Actor)
            .WithMany(p => p.PingsActor)
            .HasForeignKey(p => p.ActorUri);
        modelBuilder
            .Entity<Ping>()
            .HasOne(p => p.To)
            .WithMany(p => p.PingsTo)
            .HasForeignKey(p => p.ToUri);
        modelBuilder.Entity<Pong>().HasOne(p => p.Ping).WithMany(p => p.Pongs).IsRequired();

        // Build many-to-many relationships
        modelBuilder
            .Entity<Post>()
            .HasMany(p => p.Places)
            .WithMany(p => p.Posts)
            .UsingEntity("PostsPlaces");
        modelBuilder
            .Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Posts)
            .UsingEntity("PostsTags");
        modelBuilder
            .Entity<Place>()
            .HasMany(p => p.Tags)
            .WithMany(p => p.Places)
            .UsingEntity("PlacesTags");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new TimestampsInterceptor());
    }
}
