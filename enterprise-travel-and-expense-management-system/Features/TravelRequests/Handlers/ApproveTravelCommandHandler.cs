using MediatR;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Notifications;
using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Handlers;

/// <summary>
/// Handler for ApproveTravelCommand.
/// Updates the travel request status to Approved and publishes a TravelApprovedNotification.
/// </summary>
public class ApproveTravelCommandHandler : IRequestHandler<ApproveTravelCommand, int>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMediator _mediator;

    public ApproveTravelCommandHandler(ApplicationDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the approval of a travel request.
    /// Updates the status in the database and publishes the TravelApprovedNotification.
    /// </summary>
    public async Task<int> Handle(ApproveTravelCommand request, CancellationToken cancellationToken)
    {
        // Find the travel request by ID
        var travelRequest = await _dbContext.TravelRequests.FindAsync(
            new object[] { request.TravelRequestId },
            cancellationToken: cancellationToken
        );

        if (travelRequest == null)
        {
            throw new InvalidOperationException($"Travel request with ID {request.TravelRequestId} not found.");
        }

        // Update the status to Approved
        travelRequest.Status = TravelRequestStatus.Approved;

        // Save changes to the database
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Publish the notification
        await _mediator.Publish(
            new TravelApprovedNotification(travelRequest.Id),
            cancellationToken
        );

        return travelRequest.Id;
    }
}
