using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Models.DTOs;

/// <summary>
/// Data Transfer Object for TravelRequest, containing only essential fields for API responses.
/// </summary>
public class TravelRequestDto
{
    /// <summary>
    /// Unique identifier for the travel request.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Destination of the travel request.
    /// </summary>
    public string Destination { get; set; } = null!;

    /// <summary>
    /// Start date of the travel.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the travel.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Current status of the travel request.
    /// </summary>
    public TravelRequestStatus Status { get; set; }

    /// <summary>
    /// ID of the user who requested the travel.
    /// </summary>
    public int UserId { get; set; }
}
