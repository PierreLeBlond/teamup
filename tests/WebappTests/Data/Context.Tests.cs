using Webapp.Models;

namespace Webapp.Tests.Data;

public class ContextTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateContext()
    {
        using var context = TestDatabaseFixture.CreateContext();

        Assert.NotNull(context);
    }

    [Fact]
    public void ShouldCreateTournament()
    {
        using var context = TestDatabaseFixture.CreateContext();

        context.Tournaments.Add(new Tournament { Name = "Tournament" });
        context.SaveChanges();
    }
}
