using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using enterprise_travel_and_expense_management_system.Data;

namespace enterprise_travel_and_expense_management_system.Tests.Integration;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Configures a test server with InMemory database for isolated testing.
/// </summary>
public class TravelExpenseWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Database name shared across all clients created by this factory instance.
    /// Ensures data persistence across multiple HTTP calls in the same test.
    /// </summary>
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    /// <summary>
    /// Configures the web host with test-specific services and InMemory database.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the DbContext option registration by checking the full type name
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.FullName != null &&
                           d.ServiceType.FullName.Contains("DbContextOptions"))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database using the same database name for all clients
            // This ensures data created in one request is visible in subsequent requests
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: _databaseName));
        });

        base.ConfigureWebHost(builder);
    }
}
