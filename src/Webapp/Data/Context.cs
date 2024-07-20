using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Data;

public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<Tournament> Tournaments => Set<Tournament>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Tournament>()
            .HasData(
                new Tournament { Name = "Tournament 1" },
                new Tournament { Name = "Tournament 2" },
                new Tournament { Name = "Tournament 3" }
            );
    }
}
