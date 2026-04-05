namespace enterprise_travel_and_expense_management_system.Services;

/// <summary>
/// Service for JWT token generation.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userEmail">The user email.</param>
    /// <param name="userRole">The user role (e.g., "Employee", "Manager").</param>
    /// <returns>JWT token string.</returns>
    string GenerateToken(int userId, string userEmail, string userRole);
}
