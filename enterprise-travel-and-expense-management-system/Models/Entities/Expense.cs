namespace enterprise_travel_and_expense_management_system.Models.Entities;

public class Expense
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "USD";

    public string Description { get; set; } = null!;

    /// <summary>
    /// Path to the receipt file (relative to wwwroot).
    /// </summary>
    public string? ReceiptFilePath { get; set; }

    // Foreign key
    public int TravelRequestId { get; set; }

    // Navigation property
    public TravelRequest TravelRequest { get; set; } = null!;
}

