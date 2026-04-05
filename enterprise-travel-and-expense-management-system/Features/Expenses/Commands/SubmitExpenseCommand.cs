using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.Expenses.Commands;

/// <summary>
/// Command to submit an expense for an approved travel request.
/// </summary>
public class SubmitExpenseCommand : IRequest<int>
{
    /// <summary>
    /// The ID of the travel request to associate with this expense.
    /// </summary>
    public int TravelRequestId { get; set; }

    /// <summary>
    /// The amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The currency of the expense (default: USD).
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Description of the expense.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Base64-encoded receipt file content (optional).
    /// Can be any file format (PDF, PNG, JPG, etc).
    /// </summary>
    public string? ReceiptBase64 { get; set; }

    /// <summary>
    /// Receipt file name (e.g., "receipt.pdf" or "invoice.png"). Required if ReceiptBase64 is provided.
    /// </summary>
    public string? ReceiptFileName { get; set; }
}

