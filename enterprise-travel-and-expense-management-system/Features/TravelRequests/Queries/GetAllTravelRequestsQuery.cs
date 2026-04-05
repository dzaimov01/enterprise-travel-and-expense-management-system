using MediatR;
using enterprise_travel_and_expense_management_system.Models.DTOs;
using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Queries;

/// <summary>
/// Query to retrieve all travel requests with optional filtering by status and date range.
/// </summary>
public class GetAllTravelRequestsQuery : IRequest<List<TravelRequestDto>>
{
    /// <summary>
    /// Optional status filter. If null, all statuses are included.
    /// </summary>
    public TravelRequestStatus? Status { get; set; }

    /// <summary>
    /// Optional start date filter. If provided, returns only requests starting on or after this date.
    /// </summary>
    public DateTime? StartDateFrom { get; set; }

    /// <summary>
    /// Optional end date filter. If provided, returns only requests ending on or before this date.
    /// </summary>
    public DateTime? EndDateTo { get; set; }
}
