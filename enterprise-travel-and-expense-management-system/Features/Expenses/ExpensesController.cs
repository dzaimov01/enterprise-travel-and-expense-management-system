using MediatR;
using Microsoft.AspNetCore.Mvc;
using enterprise_travel_and_expense_management_system.Features.Expenses.Commands;

namespace enterprise_travel_and_expense_management_system.Features.Expenses;

/// <summary>
/// API Controller for managing expenses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the ExpensesController class.
    /// </summary>
    /// <param name="mediator">The MediatR mediator.</param>
    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submits a new expense for an approved travel request.
    /// </summary>
    /// <param name="command">The submit expense command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the ID of the created expense.</returns>
    /// <response code="200">Expense submitted successfully.</response>
    /// <response code="400">Invalid expense data or travel request not approved.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitExpense(
        [FromBody] SubmitExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var expenseId = await _mediator.Send(command, cancellationToken);
        return Ok(expenseId);
    }
}
