using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.UpdateSchedule;

public sealed class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CommissionId).GreaterThan(0).When(x => x.CommissionId.HasValue);
        RuleFor(x => x.Location).MaximumLength(500);
    }
}
