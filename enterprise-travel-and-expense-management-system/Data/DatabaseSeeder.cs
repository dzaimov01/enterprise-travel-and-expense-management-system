using enterprise_travel_and_expense_management_system.Models.Entities;
using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Data;

/// <summary>
/// Service for seeding initial data into the database.
/// Creates sample users, travel requests, and expenses for testing and demonstration.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with initial data if it's empty.
    /// Should be called during application startup.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger for recording seeding operations.</param>
    public static async Task SeedDatabaseAsync(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
    {
        try
        {
            // Check if database already has data
            if (context.Users.Any())
            {
                logger.LogInformation("Database already seeded. Skipping seed operation.");
                return;
            }

            logger.LogInformation("Starting database seeding...");

            // Create seed users
            var users = new List<User>
            {
                new User
                {
                    Name = "John Employee",
                    Email = "john.employee@company.com",
                    Role = UserRole.Employee
                },
                new User
                {
                    Name = "Jane Manager",
                    Email = "jane.manager@company.com",
                    Role = UserRole.Manager
                },
                new User
                {
                    Name = "Mike Employee",
                    Email = "mike.employee@company.com",
                    Role = UserRole.Employee
                },
                new User
                {
                    Name = "Admin Manager",
                    Email = "admin@company.com",
                    Role = UserRole.Manager
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            logger.LogInformation("Added {UserCount} seed users", users.Count);

            // Create seed travel requests
            var travelRequests = new List<TravelRequest>
            {
                new TravelRequest
                {
                    Destination = "New York",
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(35),
                    Status = TravelRequestStatus.Pending,
                    UserId = users[0].Id // John Employee
                },
                new TravelRequest
                {
                    Destination = "London",
                    StartDate = DateTime.UtcNow.AddDays(45),
                    EndDate = DateTime.UtcNow.AddDays(52),
                    Status = TravelRequestStatus.Approved,
                    UserId = users[2].Id // Mike Employee
                },
                new TravelRequest
                {
                    Destination = "Tokyo",
                    StartDate = DateTime.UtcNow.AddDays(60),
                    EndDate = DateTime.UtcNow.AddDays(75),
                    Status = TravelRequestStatus.Pending,
                    UserId = users[0].Id // John Employee
                }
            };

            await context.TravelRequests.AddRangeAsync(travelRequests);
            await context.SaveChangesAsync();

            logger.LogInformation("Added {TravelRequestCount} seed travel requests", travelRequests.Count);

            // Create seed expenses for approved travel requests
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Amount = 1500.00m,
                    Currency = "USD",
                    Description = "Flight ticket to London",
                    TravelRequestId = travelRequests[1].Id // London trip
                },
                new Expense
                {
                    Amount = 250.00m,
                    Currency = "GBP",
                    Description = "Hotel accommodation - 5 nights",
                    TravelRequestId = travelRequests[1].Id
                },
                new Expense
                {
                    Amount = 85.50m,
                    Currency = "GBP",
                    Description = "Meals and transportation",
                    TravelRequestId = travelRequests[1].Id
                }
            };

            await context.Expenses.AddRangeAsync(expenses);
            await context.SaveChangesAsync();

            logger.LogInformation("Added {ExpenseCount} seed expenses", expenses.Count);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database seeding");
            throw;
        }
    }
}
