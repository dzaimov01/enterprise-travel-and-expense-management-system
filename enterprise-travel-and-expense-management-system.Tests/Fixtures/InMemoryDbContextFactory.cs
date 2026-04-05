using Microsoft.EntityFrameworkCore;
using enterprise_travel_and_expense_management_system.Data;

namespace enterprise_travel_and_expense_management_system.Tests.Fixtures;

/// <summary>
/// Fixture for creating and managing InMemory database context for tests.
/// </summary>
public class InMemoryDbContextFactory
{
    /// <summary>
    /// Creates a new InMemory ApplicationDbContext for testing.
    /// </summary>
    /// <param name="databaseName">Unique database name to isolate test data.</param>
    /// <returns>ApplicationDbContext instance using InMemory provider.</returns>
    public static ApplicationDbContext CreateInMemoryContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{databaseName}_{Guid.NewGuid()}")
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
