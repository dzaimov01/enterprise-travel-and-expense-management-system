using System.Security.Claims;
using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Pipeline behavior that enforces role-based authorization for requests implementing IAuthorizedRequest.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class AuthorizerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the AuthorizerBehavior class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor for accessing user claims.</param>
    public AuthorizerBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Handles the request and enforces authorization if the request implements IAuthorizedRequest.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user lacks the required role.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Check if request requires authorization
        if (request is not IAuthorizedRequest authorizedRequest)
        {
            // No authorization required, proceed to next handler
            return await next();
        }

        // Get HTTP context
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HTTP context is not available.");
        }

        // Check if user is authenticated
        var user = httpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        // Extract user's role
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRole))
        {
            throw new UnauthorizedAccessException("User role claim is missing.");
        }

        // Check if user has required role
        if (userRole != authorizedRequest.RequiredRole)
        {
            throw new UnauthorizedAccessException(
                $"User role '{userRole}' does not have permission to execute this action. Required role: '{authorizedRequest.RequiredRole}'.");
        }

        // Authorization succeeded, proceed to next handler
        return await next();
    }
}
