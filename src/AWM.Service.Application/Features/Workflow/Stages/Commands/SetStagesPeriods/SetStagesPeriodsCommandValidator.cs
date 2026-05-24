using FluentValidation;
using AWM.Service.Domain.CommonDomain.Constants;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.SetStagesPeriods;

public sealed class SetStagesPeriodsCommandValidator : AbstractValidator<SetStagesPeriodsCommand>
{
    public SetStagesPeriodsCommandValidator()
    {
        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");

        RuleFor(x => x.Periods)
            .NotEmpty().WithMessage("Periods list cannot be empty.");

        RuleForEach(x => x.Periods).ChildRules(p =>
        {
            p.RuleFor(x => x.WorkflowStageId)
                .InclusiveBetween(WorkflowStageIds.TopicProposal, WorkflowStageIds.Preparation)
                .WithMessage($"WorkflowStageId must be between {WorkflowStageIds.TopicProposal} and {WorkflowStageIds.Preparation}.");

            p.RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            p.RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be greater than StartDate.");
        });
    }
}
