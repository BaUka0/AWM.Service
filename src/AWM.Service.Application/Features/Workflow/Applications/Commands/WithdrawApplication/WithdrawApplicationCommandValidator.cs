using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.WithdrawApplication;

public sealed class WithdrawApplicationCommandValidator : AbstractValidator<WithdrawApplicationCommand>
{
    public WithdrawApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId).GreaterThan(0);
    }
}
