namespace AWM.Service.Application.Features.Defense.Schedule.Commands.AssignWorkToSlot;

using FluentValidation;

/// <summary>
/// Validator for AssignWorkToSlotCommand.
/// </summary>
public sealed class AssignWorkToSlotCommandValidator : AbstractValidator<AssignWorkToSlotCommand>
{
    public AssignWorkToSlotCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0)
            .WithMessage("Schedule ID must be greater than 0.");

        RuleFor(x => x.WorkId)
            .GreaterThan(0)
            .WithMessage("Work ID must be greater than 0.");
    }
}
