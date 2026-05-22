using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Works.Commands.AssignReviewerToWork;

/// <summary>
/// Validator for AssignReviewerToWorkCommand.
/// </summary>
public class AssignReviewerToWorkCommandValidator : AbstractValidator<AssignReviewerToWorkCommand>
{
    public AssignReviewerToWorkCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("WorkId должен быть больше 0.");

        RuleFor(x => x.ReviewerId)
            .GreaterThan(0).WithMessage("ReviewerId должен быть больше 0.");
    }
}
