namespace Webapp.Tests.Data;

public class ContextTest(TestDatabaseFixture fixture) : IClassFixture<TestDatabaseFixture>
{
    public TestDatabaseFixture Fixture { get; } = fixture;

    [Fact]
    public void ShouldCreateContext()
    {
        using var context = Fixture.CreateContext();

        Assert.NotNull(context);
    }
}
