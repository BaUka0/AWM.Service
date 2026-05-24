using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;

public sealed class SubmitTopicsCommandValidator : AbstractValidator<SubmitTopicsCommand>
{
    public SubmitTopicsCommandValidator()
    {
        RuleFor(x => x.TopicIds).NotEmpty().WithMessage("At least one topic ID must be provided.");
        RuleForEach(x => x.TopicIds).GreaterThan(0);
    }
}
