using MediatR;
using Microsoft.AspNetCore.Mvc;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests;

/// <summary>
/// API Controller for managing travel requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TravelRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TravelRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new travel request.
    /// </summary>
    /// <param name="command">The travel request creation command.</param>
    /// <returns>Returns the ID of the created travel request.</returns>
    /// <response code="200">Travel request created successfully.</response>
    /// <response code="400">Invalid travel request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTravelRequest(
        [FromBody] CreateTravelRequestCommand command,
        CancellationToken cancellationToken)
    {
        var travelRequestId = await _mediator.Send(command, cancellationToken);
        return Ok(travelRequestId);
    }
}
