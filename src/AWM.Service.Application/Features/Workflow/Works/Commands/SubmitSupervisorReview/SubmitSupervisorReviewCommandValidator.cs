using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;

public sealed class SubmitSupervisorReviewCommandValidator : AbstractValidator<SubmitSupervisorReviewCommand>
{
    public SubmitSupervisorReviewCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty().WithMessage("File name is required.");
        RuleFor(x => x.FileStream).NotNull().WithMessage("File content is required.");
    }
}
