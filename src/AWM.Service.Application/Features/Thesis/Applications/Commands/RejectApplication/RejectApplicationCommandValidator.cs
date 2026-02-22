namespace AWM.Service.Application.Features.Thesis.Applications.Commands.RejectApplication;

using FluentValidation;

/// <summary>
/// Validator for RejectApplicationCommand.
/// </summary>
public sealed class RejectApplicationCommandValidator : AbstractValidator<RejectApplicationCommand>
{
    public RejectApplicationCommandValidator()
    {
        RuleFor(x => x.ApplicationId)
            .GreaterThan(0)
            .WithMessage("ApplicationId must be greater than 0.");

        RuleFor(x => x.SupervisorId)
            .GreaterThan(0)
            .WithMessage("SupervisorId must be greater than 0.");

        RuleFor(x => x.RejectReason)
            .NotEmpty()
            .WithMessage("Rejection reason is required.")
            .MaximumLength(1000)
            .WithMessage("Rejection reason cannot exceed 1000 characters.");
    }
}