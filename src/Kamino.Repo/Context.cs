using Kamino.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kamino.Repo;

public abstract class Context : DbContext
{
    public Context() { }

    public Context(DbContextOptions options) : base(options) { }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Build base models
        modelBuilder.Entity<Profile>().IsBasicEntity();
        modelBuilder.Entity<Post>().IsBasicEntity();
        modelBuilder.Entity<Place>().IsBasicEntity();
        modelBuilder.Entity<Tag>().IsIdentifiableEntity();

        // Build one-to-many relationships
        modelBuilder.Entity<Post>().HasOne(p => p.Author).WithMany(p => p.PostsAuthored).IsRequired();
        modelBuilder.Entity<Place>().HasOne(p => p.Author).WithMany(p => p.PlacesAuthored).IsRequired();

        // Build many-to-many relationships
        modelBuilder.Entity<Post>().HasMany(p => p.Places).WithMany(p => p.Posts).UsingEntity("PostsPlaces");
        modelBuilder.Entity<Post>().HasMany(p => p.Tags).WithMany(p => p.Posts).UsingEntity("PostsTags");
        modelBuilder.Entity<Place>().HasMany(p => p.Tags).WithMany(p => p.Places).UsingEntity("PlacesTags");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new TimestampsInterceptor());
    }
}
