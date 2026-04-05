using MediatR;
using Microsoft.EntityFrameworkCore;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Queries;
using enterprise_travel_and_expense_management_system.Models.DTOs;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Handlers;

/// <summary>
/// Handler for GetAllTravelRequestsQuery.
/// Retrieves all travel requests with optional filtering by status and date range.
/// </summary>
public class GetAllTravelRequestsQueryHandler : IRequestHandler<GetAllTravelRequestsQuery, List<TravelRequestDto>>
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the GetAllTravelRequestsQueryHandler class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public GetAllTravelRequestsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the GetAllTravelRequestsQuery by retrieving and filtering travel requests.
    /// </summary>
    /// <param name="request">The query with optional filters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of filtered TravelRequestDto objects.</returns>
    public async Task<List<TravelRequestDto>> Handle(GetAllTravelRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.TravelRequests.AsQueryable();

        // Filter by status if provided
        if (request.Status.HasValue)
        {
            query = query.Where(tr => tr.Status == request.Status.Value);
        }

        // Filter by start date if provided
        if (request.StartDateFrom.HasValue)
        {
            query = query.Where(tr => tr.StartDate >= request.StartDateFrom.Value);
        }

        // Filter by end date if provided
        if (request.EndDateTo.HasValue)
        {
            query = query.Where(tr => tr.EndDate <= request.EndDateTo.Value);
        }

        var travelRequests = await query.ToListAsync(cancellationToken);

        return travelRequests
            .Select(tr => new TravelRequestDto
            {
                Id = tr.Id,
                Destination = tr.Destination,
                StartDate = tr.StartDate,
                EndDate = tr.EndDate,
                Status = tr.Status,
                UserId = tr.UserId
            })
            .ToList();
    }
}
