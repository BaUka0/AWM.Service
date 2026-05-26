using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.CompleteQualityCheck;

public sealed class CompleteQualityCheckCommandValidator : AbstractValidator<CompleteQualityCheckCommand>
{
    public CompleteQualityCheckCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.CheckId).GreaterThan(0);
        RuleFor(x => x.ResultValue)
            .InclusiveBetween(0, 100)
            .When(x => x.ResultValue.HasValue);
    }
}
