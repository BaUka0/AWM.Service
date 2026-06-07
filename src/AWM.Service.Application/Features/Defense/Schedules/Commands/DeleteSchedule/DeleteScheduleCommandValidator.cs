using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.DeleteSchedule;

/// <summary>
/// Validator for DeleteScheduleCommand.
/// </summary>
public sealed class DeleteScheduleCommandValidator : AbstractValidator<DeleteScheduleCommand>
{
    public DeleteScheduleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
