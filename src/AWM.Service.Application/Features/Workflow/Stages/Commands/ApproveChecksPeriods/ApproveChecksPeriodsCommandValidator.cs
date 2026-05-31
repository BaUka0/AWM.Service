using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.ApproveChecksPeriods;

public sealed class ApproveChecksPeriodsCommandValidator : AbstractValidator<ApproveChecksPeriodsCommand>
{
    public ApproveChecksPeriodsCommandValidator()
    {
        RuleFor(x => x.SemesterId).GreaterThan(0);
        RuleFor(x => x.Periods).NotEmpty();
        RuleForEach(x => x.Periods).ChildRules(p =>
        {
            p.RuleFor(x => x.WorkflowStageId).GreaterThan(0);
            p.RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
        });
    }
}
