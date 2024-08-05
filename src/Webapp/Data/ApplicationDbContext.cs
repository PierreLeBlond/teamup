using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Webapp.Models;

namespace Webapp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Reward> Rewards => Set<Reward>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasIndex(p => new { p.Name, p.TournamentId }).IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
