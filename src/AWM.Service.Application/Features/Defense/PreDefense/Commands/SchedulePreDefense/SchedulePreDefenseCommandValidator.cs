namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.SchedulePreDefense;

using FluentValidation;

/// <summary>
/// Validator for SchedulePreDefenseCommand.
/// </summary>
public sealed class SchedulePreDefenseCommandValidator : AbstractValidator<SchedulePreDefenseCommand>
{
    public SchedulePreDefenseCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");

        RuleFor(x => x.WorkId)
            .GreaterThan(0)
            .WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.DefenseDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Defense date must be in the future.");

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .WithMessage("Location must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));
    }
}
