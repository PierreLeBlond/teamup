using Microsoft.EntityFrameworkCore;
using Webapp.Data;

namespace Webapp.Tests.Data;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Data Source=..\..\..\src\webapp\webapp.db";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public static Context CreateContext() =>
        new(new DbContextOptionsBuilder<Context>().UseSqlite(ConnectionString).Options);
}
