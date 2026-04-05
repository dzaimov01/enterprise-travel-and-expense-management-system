using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Notifications;

/// <summary>
/// Notification handler that sends an email when a travel request is approved.
/// This handler runs in parallel with other notification handlers.
/// </summary>
public class SendEmailOnApprovalHandler : INotificationHandler<TravelApprovedNotification>
{
    /// <summary>
    /// Handles the TravelApprovedNotification by sending an email.
    /// </summary>
    public Task Handle(TravelApprovedNotification notification, CancellationToken cancellationToken)
    {
        // Simulate sending an email
        Console.WriteLine($"[EMAIL SERVICE] Email sent for travel request ID: {notification.TravelRequestId}");
        return Task.CompletedTask;
    }
}
