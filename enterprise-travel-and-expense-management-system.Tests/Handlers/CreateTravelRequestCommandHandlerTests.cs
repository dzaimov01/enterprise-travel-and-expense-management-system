using Xunit;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Handlers;
using enterprise_travel_and_expense_management_system.Models.Entities;
using enterprise_travel_and_expense_management_system.Models.Enums;
using enterprise_travel_and_expense_management_system.Tests.Fixtures;

namespace enterprise_travel_and_expense_management_system.Tests.Handlers;

/// <summary>
/// Unit tests for CreateTravelRequestCommandHandler.
/// Tests command execution, database persistence, and error handling.
/// </summary>
public class CreateTravelRequestCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTravelRequest()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateInMemoryContext(nameof(Handle_WithValidCommand_ShouldCreateTravelRequest));
        var handler = new CreateTravelRequestCommandHandler(context);

        var command = new CreateTravelRequestCommand
        {
            Destination = "Paris",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0, "Travel request ID should be greater than 0");

        var savedRequest = await context.TravelRequests.FindAsync(result);
        Assert.NotNull(savedRequest);
        Assert.Equal("Paris", savedRequest.Destination);
        Assert.Equal(command.StartDate, savedRequest.StartDate);
        Assert.Equal(command.EndDate, savedRequest.EndDate);
        Assert.Equal(1, savedRequest.UserId);
        Assert.Equal(TravelRequestStatus.Pending, savedRequest.Status);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldSetStatusToPending()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateInMemoryContext(nameof(Handle_WithValidCommand_ShouldSetStatusToPending));
        var handler = new CreateTravelRequestCommandHandler(context);

        var command = new CreateTravelRequestCommand
        {
            Destination = "London",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(7),
            UserId = 2
        };

        // Act
        var travelRequestId = await handler.Handle(command, CancellationToken.None);

        // Assert
        var savedRequest = await context.TravelRequests.FindAsync(travelRequestId);
        Assert.NotNull(savedRequest);
        Assert.Equal(TravelRequestStatus.Pending, savedRequest.Status);
    }

    [Fact]
    public async Task Handle_MultipleCommands_ShouldGenerateUniqueIds()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateInMemoryContext(nameof(Handle_MultipleCommands_ShouldGenerateUniqueIds));
        var handler = new CreateTravelRequestCommandHandler(context);

        var command1 = new CreateTravelRequestCommand
        {
            Destination = "Berlin",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(5),
            UserId = 1
        };

        var command2 = new CreateTravelRequestCommand
        {
            Destination = "Amsterdam",
            StartDate = DateTime.UtcNow.AddDays(4),
            EndDate = DateTime.UtcNow.AddDays(8),
            UserId = 2
        };

        // Act
        var id1 = await handler.Handle(command1, CancellationToken.None);
        var id2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.Equal(2, context.TravelRequests.Count());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistToDatabase()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateInMemoryContext(nameof(Handle_WithValidCommand_ShouldPersistToDatabase));
        var handler = new CreateTravelRequestCommandHandler(context);

        var command = new CreateTravelRequestCommand
        {
            Destination = "Tokyo",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(20),
            UserId = 3
        };

        // Act
        var travelRequestId = await handler.Handle(command, CancellationToken.None);

        // Assert - Verify data is persisted within the same context
        // (InMemory database isolation means we must verify within same context)
        var persistedRequest = await context.TravelRequests.FindAsync(travelRequestId);
        Assert.NotNull(persistedRequest);
        Assert.Equal("Tokyo", persistedRequest.Destination);
        Assert.Equal(3, persistedRequest.UserId);
    }

    [Fact]
    public async Task Handle_WithDifferentUsers_ShouldPreserveUserIds()
    {
        // Arrange
        using var context = InMemoryDbContextFactory.CreateInMemoryContext(nameof(Handle_WithDifferentUsers_ShouldPreserveUserIds));
        var handler = new CreateTravelRequestCommandHandler(context);

        var command1 = new CreateTravelRequestCommand
        {
            Destination = "Rome",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(4),
            UserId = 100
        };

        var command2 = new CreateTravelRequestCommand
        {
            Destination = "Madrid",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(6),
            UserId = 200
        };

        // Act
        var id1 = await handler.Handle(command1, CancellationToken.None);
        var id2 = await handler.Handle(command2, CancellationToken.None);

        // Assert
        var request1 = await context.TravelRequests.FindAsync(id1);
        var request2 = await context.TravelRequests.FindAsync(id2);

        Assert.NotNull(request1);
        Assert.NotNull(request2);
        Assert.Equal(100, request1.UserId);
        Assert.Equal(200, request2.UserId);
    }
}
