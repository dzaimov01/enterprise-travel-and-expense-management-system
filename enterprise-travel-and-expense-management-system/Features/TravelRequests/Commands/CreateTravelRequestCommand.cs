using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;

/// <summary>
/// Command to create a new travel request for an employee.
/// Returns the ID of the created travel request.
/// </summary>
public class CreateTravelRequestCommand : IRequest<int>
{
    /// <summary>
    /// The travel destination.
    /// </summary>
    public string Destination { get; set; } = null!;

    /// <summary>
    /// The start date of the trip.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the trip.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The ID of the user requesting the travel.
    /// </summary>
    public int UserId { get; set; }
}
