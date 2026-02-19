namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;

using FluentValidation;

/// <summary>
/// Validator for RecordCheckResultCommand.
/// </summary>
public sealed class RecordCheckResultCommandValidator : AbstractValidator<RecordCheckResultCommand>
{
    public RecordCheckResultCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0)
            .WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.CheckType)
            .IsInEnum()
            .WithMessage("Check type must be a valid value (NormControl, SoftwareCheck, AntiPlagiarism).");

        RuleFor(x => x.ResultValue)
            .InclusiveBetween(0m, 100m)
            .WithMessage("Result value (plagiarism percentage) must be between 0 and 100.")
            .When(x => x.ResultValue.HasValue);

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));

        RuleFor(x => x.DocumentPath)
            .MaximumLength(1000)
            .WithMessage("Document path must not exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.DocumentPath));
    }
}
