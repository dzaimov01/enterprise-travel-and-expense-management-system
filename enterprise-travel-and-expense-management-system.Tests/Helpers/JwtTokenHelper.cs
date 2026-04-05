using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace enterprise_travel_and_expense_management_system.Tests.Helpers;

/// <summary>
/// Helper class for generating JWT tokens for test requests.
/// </summary>
public static class JwtTokenHelper
{
    private const string SecretKey = "your-super-secret-key-min-32-characters-long!";
    private const string Issuer = "TravelExpenseApp";
    private const string Audience = "TravelExpenseAppUsers";

    /// <summary>
    /// Generates a JWT token for tests with the specified role.
    /// </summary>
    /// <param name="userId">The user ID to include in the token.</param>
    /// <param name="userEmail">The user email to include in the token.</param>
    /// <param name="role">The user role ("Employee" or "Manager").</param>
    /// <returns>JWT token string ready for Authorization header.</returns>
    public static string GenerateTestToken(int userId, string userEmail, string role)
    {
        var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
