using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Models.Entities;

public class TravelRequest
{
    public int Id { get; set; }

    public string Destination { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TravelRequestStatus Status { get; set; }

    // Foreign key
    public int UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = [];
}
