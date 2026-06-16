using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.RejectApplication;

public sealed class RejectApplicationCommandValidator : AbstractValidator<RejectApplicationCommand>
{
    public RejectApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
        RuleFor(x => x.Reason).MaximumLength(2000).When(x => x.Reason != null);
    }
}
