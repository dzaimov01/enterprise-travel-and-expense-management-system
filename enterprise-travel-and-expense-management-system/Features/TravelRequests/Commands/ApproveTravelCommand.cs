using MediatR;
using enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;

/// <summary>
/// Command to approve a travel request.
/// Returns the ID of the approved travel request.
/// Only users with "Manager" role can execute this command.
/// </summary>
public class ApproveTravelCommand : IRequest<int>, IAuthorizedRequest
{
    /// <summary>
    /// The ID of the travel request to approve.
    /// </summary>
    public int TravelRequestId { get; set; }

    /// <summary>
    /// The required role to execute this command.
    /// </summary>
    public string RequiredRole => "Manager";

    public ApproveTravelCommand(int travelRequestId)
    {
        TravelRequestId = travelRequestId;
    }
}
