namespace enterprise_travel_and_expense_management_system.Models.DTOs;

/// <summary>
/// Data Transfer Object for Expense.
/// </summary>
public class ExpenseDto
{
    /// <summary>
    /// Unique identifier for the expense.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Expense amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., USD, EUR).
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Expense description.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Receipt file path (if available).
    /// </summary>
    public string? ReceiptFilePath { get; set; }

    /// <summary>
    /// Associated travel request ID.
    /// </summary>
    public int TravelRequestId { get; set; }
}
