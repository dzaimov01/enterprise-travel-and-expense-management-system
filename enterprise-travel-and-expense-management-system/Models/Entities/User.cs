using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Models.Entities;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }

    // Navigation property
    public ICollection<TravelRequest> TravelRequests { get; set; } = [];
}
