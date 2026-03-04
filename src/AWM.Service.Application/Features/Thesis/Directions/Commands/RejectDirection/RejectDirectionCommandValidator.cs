namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RejectDirection;

using FluentValidation;

/// <summary>
/// Validator for RejectDirectionCommand.
/// </summary>
public sealed class RejectDirectionCommandValidator
    : AbstractValidator<RejectDirectionCommand>
{
    public RejectDirectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Direction ID must be greater than 0.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Rejection comment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Comment));

        // Optional: Enforce comment requirement
        // RuleFor(x => x.Comment)
        //     .NotEmpty()
        //     .WithMessage("Rejection comment is required to provide feedback to the supervisor.");
    }
}