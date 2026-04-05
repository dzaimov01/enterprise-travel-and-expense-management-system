using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace enterprise_travel_and_expense_management_system.Services;

/// <summary>
/// Service for generating JWT tokens.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the JwtTokenService class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="userEmail">The user email.</param>
    /// <param name="userRole">The user role (e.g., "Employee", "Manager").</param>
    /// <returns>JWT token string.</returns>
    public string GenerateToken(int userId, string userEmail, string userRole)
    {
        var key = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured in appsettings.json");
        var issuer = _configuration["Jwt:Issuer"] ?? "TravelExpenseApp";
        var audience = _configuration["Jwt:Audience"] ?? "TravelExpenseAppUsers";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, userEmail),
            new Claim(ClaimTypes.Role, userRole)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
