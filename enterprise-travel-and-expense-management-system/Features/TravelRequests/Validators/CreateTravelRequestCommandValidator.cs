using FluentValidation;
using enterprise_travel_and_expense_management_system.Features.TravelRequests.Commands;

namespace enterprise_travel_and_expense_management_system.Features.TravelRequests.Validators;

/// <summary>
/// Validator for CreateTravelRequestCommand using FluentValidation.
/// Ensures all travel request data is valid before processing.
/// </summary>
public class CreateTravelRequestCommandValidator : AbstractValidator<CreateTravelRequestCommand>
{
    public CreateTravelRequestCommandValidator()
    {
        // Destination validation
        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination cannot be empty.")
            .MaximumLength(256)
            .WithMessage("Destination must not exceed 256 characters.");

        // StartDate validation
        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Start date must be in the future.");

        // EndDate validation
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");

        // UserId validation
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be a valid positive number.");
    }
}
