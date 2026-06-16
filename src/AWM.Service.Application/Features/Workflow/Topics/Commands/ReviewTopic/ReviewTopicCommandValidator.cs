using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReviewTopic;

public sealed class ReviewTopicCommandValidator : AbstractValidator<ReviewTopicCommand>
{
    public ReviewTopicCommandValidator()
    {
        RuleFor(x => x.TopicId).GreaterThan(0);
        RuleFor(x => x.Comment).NotEmpty().When(x => !x.IsApproved).WithMessage("Comment is required for rejection.");
    }
}
