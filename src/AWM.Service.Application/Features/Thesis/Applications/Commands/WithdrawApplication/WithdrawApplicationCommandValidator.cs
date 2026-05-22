using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;

/// <summary>
/// Validator for WithdrawApplicationCommand.
/// </summary>
public class WithdrawApplicationCommandValidator : AbstractValidator<WithdrawApplicationCommand>
{
    public WithdrawApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0).WithMessage("ApplicationId должен быть больше 0.");
    }
}
