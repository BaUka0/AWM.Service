namespace AWM.Service.Application.Features.Defense.Schedule.Commands.UpdateDefenseSchedule;

using FluentValidation;

/// <summary>
/// Validator for UpdateDefenseScheduleCommand.
/// </summary>
public sealed class UpdateDefenseScheduleCommandValidator : AbstractValidator<UpdateDefenseScheduleCommand>
{
    public UpdateDefenseScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0)
            .WithMessage("Schedule ID must be greater than 0.");

        RuleFor(x => x.DefenseDate)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage("Defense date must be in the future.")
            .When(x => x.DefenseDate.HasValue);

        RuleFor(x => x.Location)
            .MaximumLength(500)
            .WithMessage("Location must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Location));
    }
}
