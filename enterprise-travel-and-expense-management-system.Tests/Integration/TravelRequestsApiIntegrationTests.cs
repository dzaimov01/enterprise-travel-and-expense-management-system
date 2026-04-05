using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;
using enterprise_travel_and_expense_management_system.Tests.Helpers;
using enterprise_travel_and_expense_management_system.Tests.Integration;

namespace enterprise_travel_and_expense_management_system.Tests.Integration;

/// <summary>
/// Integration tests for TravelRequests API endpoints.
/// Tests the full cycle: HTTP request → validation → handler → database → response.
/// </summary>
public class TravelRequestsApiIntegrationTests : IClassFixture<TravelExpenseWebApplicationFactory>
{
    private readonly TravelExpenseWebApplicationFactory _factory;

    public TravelRequestsApiIntegrationTests(TravelExpenseWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTravelRequest_WithValidData_ShouldReturnCreatedRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var command = new CreateTravelRequestCommand
        {
            Destination = "Paris",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var resultJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<int>(resultJson);
        Assert.True(result > 0, "Should return a positive travel request ID");
    }

    [Fact]
    public async Task CreateTravelRequest_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        // No Authorization header

        var command = new CreateTravelRequestCommand
        {
            Destination = "London",
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(7),
            UserId = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTravelRequest_WithPastStartDate_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var command = new CreateTravelRequestCommand
        {
            Destination = "Berlin",
            StartDate = DateTime.UtcNow.AddDays(-1), // Past date - invalid
            EndDate = DateTime.UtcNow.AddDays(3),
            UserId = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("StartDate", errorContent);
    }

    [Fact]
    public async Task CreateTravelRequest_WithEndDateBeforeStartDate_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var startDate = DateTime.UtcNow.AddDays(10);
        var endDate = DateTime.UtcNow.AddDays(5); // Before start date - invalid

        var command = new CreateTravelRequestCommand
        {
            Destination = "Amsterdam",
            StartDate = startDate,
            EndDate = endDate,
            UserId = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("EndDate", errorContent);
    }

    [Fact]
    public async Task CreateTravelRequest_WithEmptyDestination_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var command = new CreateTravelRequestCommand
        {
            Destination = "", // Empty - invalid
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTravelRequest_WithInvalidUserId_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var command = new CreateTravelRequestCommand
        {
            Destination = "Rome",
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(10),
            UserId = 0 // Invalid - must be > 0
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/travelrequests", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTravelRequestById_WithValidId_ShouldReturnTravelRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // First, create a travel request
        var createCommand = new CreateTravelRequestCommand
        {
            Destination = "Tokyo",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(20),
            UserId = 1
        };

        var createResponse = await client.PostAsJsonAsync("/api/travelrequests", createCommand);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var createResponseJson = await createResponse.Content.ReadAsStringAsync();
        var travelRequestId = int.TryParse(createResponseJson.Trim('"'), out var id) ? id : 0;
        Assert.True(travelRequestId > 0, $"Travel request ID should be greater than 0, got: {createResponseJson}");

        // Act - Get the created travel request
        var getResponse = await client.GetAsync($"/api/travelrequests/{travelRequestId}");

        // Assert - if got 500, read the error
        if (getResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            var errorContent = await getResponse.Content.ReadAsStringAsync();
            Assert.Fail($"Got 500 error: {errorContent}");
        }

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var travelRequestJson = await getResponse.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(travelRequestJson);
        var root = jsonDoc.RootElement;

        var retrievedId = root.GetProperty("id").GetInt32();
        var destination = root.GetProperty("destination").GetString();

        Assert.Equal(travelRequestId, retrievedId);
        Assert.Equal("Tokyo", destination);
    }

    [Fact]
    public async Task GetAllTravelRequests_WithValidToken_ShouldReturnList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var token = JwtTokenHelper.GenerateTestToken(1, "employee@test.com", "Employee");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        // Create multiple travel requests and track their IDs
        var createdIds = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            var command = new CreateTravelRequestCommand
            {
                Destination = $"City{i}",
                StartDate = DateTime.UtcNow.AddDays(5 + i),
                EndDate = DateTime.UtcNow.AddDays(10 + i),
                UserId = 1
            };
            var createResponse = await client.PostAsJsonAsync("/api/travelrequests", command);
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

            var createResponseJson = await createResponse.Content.ReadAsStringAsync();
            var travelRequestId = int.TryParse(createResponseJson.Trim('"'), out var id) ? id : 0;
            Assert.True(travelRequestId > 0, $"Failed to parse travel request ID from: {createResponseJson}");
            createdIds.Add(travelRequestId);
        }

        // Act
        var response = await client.GetAsync("/api/travelrequests");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var travelRequestsJson = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        using var jsonDoc = JsonDocument.Parse(travelRequestsJson);
        var root = jsonDoc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        var retrievedIds = root.EnumerateArray()
            .Select(element => element.GetProperty("id").GetInt32())
            .ToList();

        // Verify all created IDs are in the response
        foreach (var createdId in createdIds)
        {
            Assert.Contains(createdId, retrievedIds);
        }
    }
}
