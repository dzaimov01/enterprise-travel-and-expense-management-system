using MediatR;
using Microsoft.EntityFrameworkCore;
using enterprise_travel_and_expense_management_system.Data;
using enterprise_travel_and_expense_management_system.Features.Expenses.Commands;
using enterprise_travel_and_expense_management_system.Models.Entities;
using enterprise_travel_and_expense_management_system.Models.Enums;
using enterprise_travel_and_expense_management_system.Services;

namespace enterprise_travel_and_expense_management_system.Features.Expenses.Handlers;

/// <summary>
/// Handler for SubmitExpenseCommand.
/// Creates a new expense for an approved travel request with optional receipt attachment.
/// </summary>
public class SubmitExpenseCommandHandler : IRequestHandler<SubmitExpenseCommand, int>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFileService _fileService;

    /// <summary>
    /// Initializes a new instance of the SubmitExpenseCommandHandler class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="fileService">The file service for handling receipt uploads.</param>
    public SubmitExpenseCommandHandler(ApplicationDbContext dbContext, IFileService fileService)
    {
        _dbContext = dbContext;
        _fileService = fileService;
    }

    /// <summary>
    /// Handles the SubmitExpenseCommand by creating a new expense for an approved travel request.
    /// </summary>
    /// <param name="command">The submit expense command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ID of the newly created expense.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the travel request is not found or is not in Approved status.
    /// </exception>
    public async Task<int> Handle(SubmitExpenseCommand command, CancellationToken cancellationToken)
    {
        // Check if travel request exists
        var travelRequest = await _dbContext.TravelRequests
            .FirstOrDefaultAsync(tr => tr.Id == command.TravelRequestId, cancellationToken);

        if (travelRequest == null)
        {
            throw new InvalidOperationException($"Travel request with ID {command.TravelRequestId} not found.");
        }

        // Verify that the travel request is in Approved status
        if (travelRequest.Status != TravelRequestStatus.Approved)
        {
            throw new InvalidOperationException(
                $"Cannot submit expense for travel request in '{travelRequest.Status}' status. " +
                $"Only 'Approved' travel requests can have expenses.");
        }

        // Handle receipt file if provided
        string? receiptFilePath = null;
        if (!string.IsNullOrEmpty(command.ReceiptBase64) && !string.IsNullOrEmpty(command.ReceiptFileName))
        {
            receiptFilePath = await _fileService.SaveReceiptFromBase64Async(
                command.ReceiptBase64,
                command.ReceiptFileName,
                cancellationToken);
        }

        // Create the expense entity
        var expense = new Expense
        {
            TravelRequestId = command.TravelRequestId,
            Amount = command.Amount,
            Currency = command.Currency,
            Description = command.Description,
            ReceiptFilePath = receiptFilePath
        };

        // Add to database
        _dbContext.Expenses.Add(expense);

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }
}

