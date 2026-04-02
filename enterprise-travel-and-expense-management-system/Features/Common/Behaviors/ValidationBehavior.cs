using FluentValidation;
using MediatR;

namespace enterprise_travel_and_expense_management_system.Features.Common.Behaviors;

/// <summary>
/// Generic pipeline behavior that validates all requests before they reach handlers.
/// Runs all registered FluentValidation validators for the request type.
/// Throws ValidationException if validation fails, preventing handler execution.
/// </summary>
/// <typeparam name="TRequest">The request type to validate.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Executes all validators for the request.
    /// If validation errors occur, throws ValidationException.
    /// Otherwise, proceeds to the next behavior/handler.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Get all validators for this request type
        var validationTasks = _validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);

        // Collect all validation failures
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        // If there are validation errors, throw ValidationException
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // If validation passes, proceed to the next behavior/handler
        return await next();
    }
}
