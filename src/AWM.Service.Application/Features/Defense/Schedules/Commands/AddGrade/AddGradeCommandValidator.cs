using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.AddGrade;

public sealed class AddGradeCommandValidator : AbstractValidator<AddGradeCommand>
{
    public AddGradeCommandValidator()
    {
        RuleFor(x => x.ScheduleId).GreaterThan(0);
        RuleFor(x => x.CriteriaId).GreaterThan(0);
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Comment).MaximumLength(2000).When(x => x.Comment != null);
    }
}
