using enterprise_travel_and_expense_management_system.Models.Enums;

namespace enterprise_travel_and_expense_management_system.Models.DTOs;

/// <summary>
/// Data Transfer Object for User.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's full name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's role (Employee or Manager).
    /// </summary>
    public UserRole Role { get; set; }
}
