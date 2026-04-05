using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Queries;
using enterprise_travel_and_expense_management_system.Models.DTOs;
using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests;

/// <summary>
/// API Controller for managing travel requests.
/// All endpoints require JWT authentication. Employees can create and view their requests,
/// managers can approve, reject, and view all requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TravelRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the TravelRequestsController class.
    /// </summary>
    /// <param name="mediator">The MediatR mediator for sending commands and queries.</param>
    public TravelRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new travel request.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/travelrequests
    ///     {
    ///       "destination": "Paris",
    ///       "startDate": "2026-05-15T00:00:00Z",
    ///       "endDate": "2026-05-20T00:00:00Z",
    ///       "userId": 1
    ///     }
    /// </remarks>
    /// <param name="command">The travel request creation command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the ID of the created travel request.</returns>
    /// <response code="200">Travel request created successfully. Returns the ID of the new travel request.</response>
    /// <response code="400">Invalid travel request data. Validation failed (empty destination, past dates, end date before start date, invalid user ID).</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="500">Internal server error occurred while creating the travel request.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTravelRequest(
        [FromBody] CreateTravelRequestCommand command,
        CancellationToken cancellationToken)
    {
        var travelRequestId = await _mediator.Send(command, cancellationToken);
        return Ok(travelRequestId);
    }

    /// <summary>
    /// Approves a travel request. Only managers can approve requests.
    /// </summary>
    /// <remarks>
    /// This endpoint changes the travel request status from Pending to Approved.
    /// A notification is sent containing the travel request ID.
    /// Only users with Manager role can call this endpoint.
    /// </remarks>
    /// <param name="travelRequestId">The ID of the travel request to approve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the ID of the approved travel request.</returns>
    /// <response code="200">Travel request approved successfully. Returns the ID of the approved request.</response>
    /// <response code="400">Invalid travel request ID or travel request is not in Pending status.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="403">Forbidden. User does not have Manager role.</response>
    /// <response code="404">Travel request with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred while approving the travel request.</response>
    [HttpPut("{travelRequestId}/approve")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveTravelRequest(
        int travelRequestId,
        CancellationToken cancellationToken)
    {
        var command = new ApproveTravelCommand(travelRequestId);
        var approvedRequestId = await _mediator.Send(command, cancellationToken);
        return Ok(approvedRequestId);
    }

    /// <summary>
    /// Retrieves all travel requests with optional filtering.
    /// </summary>
    /// <remarks>
    /// Returns a list of all travel requests that match the optional filters.
    /// Results are cached for 5 minutes for performance optimization.
    ///
    /// Query parameters:
    /// - status: Filter by travel request status (Pending, Approved, Rejected, Completed, Cancelled)
    /// - startDateFrom: Include requests starting on or after this date (ISO 8601 format)
    /// - endDateTo: Include requests ending on or before this date (ISO 8601 format)
    /// </remarks>
    /// <param name="status">Optional filter by travel request status.</param>
    /// <param name="startDateFrom">Optional filter to include requests starting on or after this date.</param>
    /// <param name="endDateTo">Optional filter to include requests ending on or before this date.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns a list of travel request DTOs matching the filters.</returns>
    /// <response code="200">Travel requests retrieved successfully. Returns an array of travel request DTOs.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="500">Internal server error occurred while retrieving travel requests.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TravelRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTravelRequests(
        [FromQuery] TravelRequestStatus? status,
        [FromQuery] DateTime? startDateFrom,
        [FromQuery] DateTime? endDateTo,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTravelRequestsQuery
        {
            Status = status,
            StartDateFrom = startDateFrom,
            EndDateTo = endDateTo
        };

        var travelRequests = await _mediator.Send(query, cancellationToken);
        return Ok(travelRequests);
    }

    /// <summary>
    /// Retrieves a specific travel request by its ID with caching.
    /// </summary>
    /// <remarks>
    /// Returns details of a single travel request as a DTO.
    /// Results are cached for 5 minutes for performance optimization.
    /// </remarks>
    /// <param name="travelRequestId">The ID of the travel request to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the travel request details as a DTO.</returns>
    /// <response code="200">Travel request retrieved successfully. Returns the travel request DTO.</response>
    /// <response code="401">Unauthorized. JWT token is missing or invalid.</response>
    /// <response code="404">Travel request with the specified ID was not found.</response>
    /// <response code="500">Internal server error occurred while retrieving the travel request.</response>
    [HttpGet("{travelRequestId}")]
    [ProducesResponseType(typeof(TravelRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTravelRequestById(
        int travelRequestId,
        CancellationToken cancellationToken)
    {
        var query = new GetTravelRequestByIdQuery(travelRequestId);
        var travelRequestDto = await _mediator.Send(query, cancellationToken);
        return Ok(travelRequestDto);
    }
}
