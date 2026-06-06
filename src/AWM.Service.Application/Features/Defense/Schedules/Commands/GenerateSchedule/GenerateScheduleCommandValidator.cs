using FluentValidation;
using System;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;

public sealed class GenerateScheduleCommandValidator : AbstractValidator<GenerateScheduleCommand>
{
    public GenerateScheduleCommandValidator()
    {
        RuleFor(x => x.CommissionId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty().NotEqual(default(DateTime));
        RuleFor(x => x.Location).MaximumLength(500).When(x => x.Location != null);
        RuleFor(x => x.SlotDurationMinutes).GreaterThan(0);
        RuleFor(x => x.WorkIds).NotEmpty();
        RuleForEach(x => x.WorkIds).GreaterThan(0);
    }
}
