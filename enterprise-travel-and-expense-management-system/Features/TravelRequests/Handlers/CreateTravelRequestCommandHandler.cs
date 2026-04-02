using MediatR;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;
using enterprise_travel_and_expense_management_system.Models.Entities;
using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Handlers;

/// <summary>
/// Handler for CreateTravelRequestCommand.
/// Persists the travel request to the database and returns its ID.
/// </summary>
public class CreateTravelRequestCommandHandler : IRequestHandler<CreateTravelRequestCommand, int>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateTravelRequestCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the creation of a new travel request.
    /// Maps command data to TravelRequest entity, saves to database, and returns the ID.
    /// </summary>
    public async Task<int> Handle(CreateTravelRequestCommand request, CancellationToken cancellationToken)
    {
        var travelRequest = new TravelRequest
        {
            Destination = request.Destination,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            UserId = request.UserId,
            Status = TravelRequestStatus.Pending
        };

        _dbContext.TravelRequests.Add(travelRequest);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return travelRequest.Id;
    }
}
