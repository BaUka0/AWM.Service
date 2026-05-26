using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AssignReviewer;

public sealed class AssignReviewerCommandValidator : AbstractValidator<AssignReviewerCommand>
{
    public AssignReviewerCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.ReviewerId).GreaterThan(0);
    }
}
