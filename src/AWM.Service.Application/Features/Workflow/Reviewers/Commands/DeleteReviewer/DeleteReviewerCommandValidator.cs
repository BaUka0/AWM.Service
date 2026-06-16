using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.DeleteReviewer;

public sealed class DeleteReviewerCommandValidator : AbstractValidator<DeleteReviewerCommand>
{
    public DeleteReviewerCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
