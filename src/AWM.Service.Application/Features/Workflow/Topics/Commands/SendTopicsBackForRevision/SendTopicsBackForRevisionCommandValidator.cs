using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SendTopicsBackForRevision;

public sealed class SendTopicsBackForRevisionCommandValidator : AbstractValidator<SendTopicsBackForRevisionCommand>
{
    public SendTopicsBackForRevisionCommandValidator()
    {
        RuleFor(x => x.TopicIds).NotEmpty().WithMessage("At least one topic must be selected.");
        RuleForEach(x => x.TopicIds).GreaterThan(0).WithMessage("Topic ID must be greater than 0.");
        RuleFor(x => x.Comment).NotEmpty().WithMessage("Comment is required when sending topics back for revision.");
    }
}
