namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Interface for requests that require authorization based on user roles.
/// </summary>
public interface IAuthorizedRequest
{
    /// <summary>
    /// The required role to execute this request.
    /// </summary>
    string RequiredRole { get; }
}
