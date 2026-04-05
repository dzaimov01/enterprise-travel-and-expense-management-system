using Microsoft.AspNetCore.Mvc;
using enterprise_travel_and_expense_management_system.Services;

namespace enterprise_travel_and_expense_management_system.Features.Auth;

/// <summary>
/// API Controller for authentication and JWT token generation.
/// Provides endpoints for user login and token acquisition.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// </summary>
    /// <param name="jwtTokenService">The JWT token service for generating tokens.</param>
    public AuthController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <remarks>
    /// Generates a JWT token for the specified user with the given role.
    /// Valid roles are "Employee" and "Manager".
    ///
    /// Sample request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///       "email": "user@example.com",
    ///       "role": "Employee"
    ///     }
    ///
    /// Returns a token that should be included in subsequent API requests
    /// using the Authorization header format: "Bearer {token}"
    /// </remarks>
    /// <param name="request">The login request containing email and role.</param>
    /// <returns>JWT token and user information for authenticated user.</returns>
    /// <response code="200">Authentication successful. Returns JWT token and user details.</response>
    /// <response code="400">Invalid request data. Email and Role are required, or Role is not valid (must be 'Employee' or 'Manager').</response>
    /// <response code="500">Internal server error occurred during token generation.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new { error = "Email and Role are required." });
        }

        // Validate role: only "Employee" or "Manager" allowed
        if (request.Role != "Employee" && request.Role != "Manager")
        {
            return BadRequest(new { error = "Role must be 'Employee' or 'Manager'." });
        }

        // For demo purposes, generate a token with userId=1. In production, validate user in database.
        var token = _jwtTokenService.GenerateToken(
            userId: 1,
            userEmail: request.Email,
            userRole: request.Role
        );

        return Ok(new LoginResponse
        {
            Token = token,
            Email = request.Email,
            Role = request.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse("60"))
        });
    }
}

/// <summary>
/// Request model for user login.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user role ("Employee" or "Manager").
    /// </summary>
    public string Role { get; set; } = null!;
}

/// <summary>
/// Response model for successful authentication.
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Gets or sets the JWT token for API access.
    /// Include this token in the Authorization header of subsequent requests.
    /// Format: "Authorization: Bearer {token}"
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user email address.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user role.
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets the token expiration time (UTC).
    /// After this time, the token will be invalid and a new login is required.
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
