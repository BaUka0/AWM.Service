namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.SubmitGrade;

using FluentValidation;

/// <summary>
/// Validator for SubmitGradeCommand.
/// </summary>
public sealed class SubmitGradeCommandValidator : AbstractValidator<SubmitGradeCommand>
{
    public SubmitGradeCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0)
            .WithMessage("Schedule ID must be greater than 0.");

        RuleFor(x => x.MemberId)
            .GreaterThan(0)
            .WithMessage("Member ID must be greater than 0.");

        RuleFor(x => x.CriteriaId)
            .GreaterThan(0)
            .WithMessage("Criteria ID must be greater than 0.");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100)
            .WithMessage("Score must be between 0 and 100.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .WithMessage("Comment must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
