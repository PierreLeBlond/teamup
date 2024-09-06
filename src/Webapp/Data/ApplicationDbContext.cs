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
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Teammate> Teammates => Set<Teammate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>().HasIndex(p => new { p.Name, p.TournamentId }).IsUnique();
        modelBuilder.Entity<Game>().HasIndex(g => new { g.Name, g.TournamentId }).IsUnique();

        modelBuilder.Entity<Tournament>().HasIndex(t => t.Name).IsUnique();

        // Kept for reference, I couldn't find a way to ensure a teamate's team and player are in the same tournament
        // This solution does not work as subqueries aren't allowed within check constraints
        /*modelBuilder
            .Entity<Teammate>()
            .ToTable(b =>
                b.HasCheckConstraint(
                    "CK_Teammate_TeamAndPlayerSameTournament",
                    "[TeamId] IN (SELECT TeamId FROM Teams JOIN Games ON Games.Id = GameId WHERE TournamentId = (SELECT TournamentId FROM Players WHERE Id = [PlayerId]))"
                )
            );*/

        base.OnModelCreating(modelBuilder);
    }
}
