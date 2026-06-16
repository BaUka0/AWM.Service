using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SubmitForCheck;

public sealed class SubmitForCheckCommandValidator : AbstractValidator<SubmitForCheckCommand>
{
    public SubmitForCheckCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.CheckTypeId).InclusiveBetween(1, 3);
    }
}
