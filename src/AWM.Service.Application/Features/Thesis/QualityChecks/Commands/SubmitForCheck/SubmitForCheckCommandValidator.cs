namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using FluentValidation;

/// <summary>
/// Validator for SubmitForCheckCommand.
/// </summary>
public sealed class SubmitForCheckCommandValidator : AbstractValidator<SubmitForCheckCommand>
{
    public SubmitForCheckCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0)
            .WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.CheckTypeId)
            .InclusiveBetween(1, 3)
            .WithMessage("Check type must be a valid value (1 = NormControl, 2 = SoftwareCheck, 3 = AntiPlagiarism).");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
