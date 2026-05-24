using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.ReviewDirection;

public sealed class ReviewDirectionCommandValidator : AbstractValidator<ReviewDirectionCommand>
{
    public ReviewDirectionCommandValidator()
    {
        RuleFor(v => v.DirectionId).GreaterThan(0);
        RuleFor(v => v.Decision).IsInEnum();
        RuleFor(v => v.Comment)
            .NotEmpty()
            .When(v => v.Decision == ReviewDecision.RequireRevision || v.Decision == ReviewDecision.Reject)
            .WithMessage("Comment is required for rejection or revision request.");
    }
}
