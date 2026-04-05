using FluentValidation;
using enterprise_travel_and_expense_management_system.Features.Expenses.Commands;

namespace enterprise_travel_and_expense_management_system.Features.Expenses.Validators;

/// <summary>
/// Validator for SubmitExpenseCommand.
/// </summary>
public class SubmitExpenseCommandValidator : AbstractValidator<SubmitExpenseCommand>
{
    /// <summary>
    /// Initializes a new instance of the SubmitExpenseCommandValidator class.
    /// </summary>
    public SubmitExpenseCommandValidator()
    {
        RuleFor(x => x.TravelRequestId)
            .GreaterThan(0)
            .WithMessage("Travel request ID must be a positive integer.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .MaximumLength(3)
            .WithMessage("Currency code must not exceed 3 characters (e.g., USD, EUR, GBP).");

        // Receipt validation: if ReceiptBase64 is provided, ReceiptFileName must also be provided
        RuleFor(x => x.ReceiptFileName)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.ReceiptBase64))
            .WithMessage("Receipt file name is required when providing a receipt file.");

        // Validate Base64 format if provided
        RuleFor(x => x.ReceiptBase64)
            .Must(IsValidBase64)
            .When(x => !string.IsNullOrEmpty(x.ReceiptBase64))
            .WithMessage("Receipt file must be in valid Base64 format.");
    }

    /// <summary>
    /// Validates if a string is a valid Base64-encoded value.
    /// </summary>
    private static bool IsValidBase64(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

