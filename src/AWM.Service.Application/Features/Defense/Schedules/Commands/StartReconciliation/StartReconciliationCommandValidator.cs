using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.StartReconciliation;

public sealed class StartReconciliationCommandValidator : AbstractValidator<StartReconciliationCommand>
{
    public StartReconciliationCommandValidator()
    {
        RuleFor(x => x.ScheduleId).GreaterThan(0);
    }
}
