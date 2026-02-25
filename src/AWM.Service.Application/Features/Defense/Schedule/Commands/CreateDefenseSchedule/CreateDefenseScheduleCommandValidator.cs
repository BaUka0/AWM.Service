namespace AWM.Service.Application.Features.Defense.Schedule.Commands.CreateDefenseSchedule;

using FluentValidation;

/// <summary>
/// Validator for CreateDefenseScheduleCommand.
/// </summary>
public sealed class CreateDefenseScheduleCommandValidator : AbstractValidator<CreateDefenseScheduleCommand>
{
    public CreateDefenseScheduleCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");

        RuleFor(x => x.DefenseDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Defense date must be in the future.");

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .WithMessage("Location must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));
    }
}
