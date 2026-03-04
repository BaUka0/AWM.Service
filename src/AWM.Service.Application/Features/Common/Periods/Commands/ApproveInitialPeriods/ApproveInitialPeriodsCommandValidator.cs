namespace AWM.Service.Application.Features.Common.Periods.Commands.ApproveInitialPeriods;

using FluentValidation;

public sealed class ApproveInitialPeriodsCommandValidator : AbstractValidator<ApproveInitialPeriodsCommand>
{
    public ApproveInitialPeriodsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be specified.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0)
            .WithMessage("Academic Year ID must be specified.");

        RuleFor(x => x.Periods)
            .NotEmpty()
            .WithMessage("At least one period must be provided.");

        RuleForEach(x => x.Periods).ChildRules(period =>
        {
            period.RuleFor(p => p.EndDate)
                .GreaterThan(p => p.StartDate)
                .WithMessage("End date must be after start date.");
        });
    }
}
