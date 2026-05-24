using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Stages.Queries.GetStagesPeriods;

public sealed class GetStagesPeriodsQueryValidator : AbstractValidator<GetStagesPeriodsQuery>
{
    public GetStagesPeriodsQueryValidator()
    {
        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");
    }
}
