using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.AcceptApplication;

public sealed class AcceptApplicationCommandValidator : AbstractValidator<AcceptApplicationCommand>
{
    public AcceptApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
    }
}
