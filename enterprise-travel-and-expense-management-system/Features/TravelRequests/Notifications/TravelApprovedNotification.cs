using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Notifications;

/// <summary>
/// Notification published when a travel request is approved.
/// </summary>
public class TravelApprovedNotification : INotification
{
    /// <summary>
    /// The ID of the approved travel request.
    /// </summary>
    public int TravelRequestId { get; set; }

    public TravelApprovedNotification(int travelRequestId)
    {
        TravelRequestId = travelRequestId;
    }
}
