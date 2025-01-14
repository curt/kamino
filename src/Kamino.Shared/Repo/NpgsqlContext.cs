using Microsoft.EntityFrameworkCore;

namespace Kamino.Shared.Repo;

public class NpgsqlContext(DbContextOptions<NpgsqlContext> options) : Context(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.HasPostgresEnum<PostType>();
        modelBuilder.HasPostgresEnum<SourceType>();

        modelBuilder.Entity<Place>(e =>
            e.Property(p => p.Location).HasColumnType("geography (point)")
        );
    }
}
