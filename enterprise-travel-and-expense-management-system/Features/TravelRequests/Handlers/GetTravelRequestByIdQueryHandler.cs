using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Queries;
using enterprise_travel_and_expense_management_system.Models.DTOs;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Handlers;

/// <summary>
/// Handler for GetTravelRequestByIdQuery.
/// Retrieves a travel request by its ID and maps it to a DTO.
/// </summary>
public class GetTravelRequestByIdQueryHandler : IRequestHandler<GetTravelRequestByIdQuery, TravelRequestDto>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the GetTravelRequestByIdQueryHandler class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="mapper">The AutoMapper instance for entity-to-DTO mapping.</param>
    public GetTravelRequestByIdQueryHandler(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetTravelRequestByIdQuery by retrieving the travel request
    /// and mapping it to a TravelRequestDto using AutoMapper.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A TravelRequestDto if found; otherwise, throws an InvalidOperationException.</returns>
    public async Task<TravelRequestDto> Handle(GetTravelRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var travelRequest = await _dbContext.TravelRequests
            .FirstOrDefaultAsync(tr => tr.Id == request.TravelRequestId, cancellationToken);

        if (travelRequest == null)
        {
            throw new InvalidOperationException($"Travel request with ID {request.TravelRequestId} not found.");
        }

        return _mapper.Map<TravelRequestDto>(travelRequest);
    }
}
