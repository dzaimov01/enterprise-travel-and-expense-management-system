using MediatR;
using enterprise_travel_and_expense_management_system.Features.Common.Behaviors;
using enterprise_travel_and_expense_management_system.Models.DTOs;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Queries;

/// <summary>
/// Query to retrieve a travel request by its ID.
/// Results are cached for 5 minutes to improve performance.
/// </summary>
public class GetTravelRequestByIdQuery : IRequest<TravelRequestDto>, ICacheable
{
    /// <summary>
    /// The ID of the travel request to retrieve.
    /// </summary>
    public int TravelRequestId { get; set; }

    /// <summary>
    /// Gets the cache key for this query.
    /// </summary>
    public string CacheKey => $"travel_request_{TravelRequestId}";

    /// <summary>
    /// Gets the cache duration in seconds (5 minutes).
    /// </summary>
    public int DurationSeconds => 300;

    /// <summary>
    /// Initializes a new instance of the GetTravelRequestByIdQuery class.
    /// </summary>
    /// <param name="travelRequestId">The ID of the travel request to retrieve.</param>
    public GetTravelRequestByIdQuery(int travelRequestId)
    {
        TravelRequestId = travelRequestId;
    }
}
