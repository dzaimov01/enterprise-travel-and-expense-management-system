using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Notifications;

/// <summary>
/// Notification handler that logs an audit entry when a travel request is approved.
/// This handler runs in parallel with other notification handlers.
/// </summary>
public class LogAuditOnApprovalHandler : INotificationHandler<TravelApprovedNotification>
{
    /// <summary>
    /// Handles the TravelApprovedNotification by creating an audit log entry.
    /// </summary>
    public Task Handle(TravelApprovedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate creating an audit log entry
        Console.WriteLine($"[AUDIT LOG] Travel request ID {notification.TravelRequestId} was approved at {DateTime.UtcNow:O}");
        return Task.CompletedTask;
    }
}
