using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CloseTopic;

/// <summary>
/// Validator for <see cref="CloseTopicCommand"/>.
/// </summary>
public sealed class CloseTopicCommandValidator : AbstractValidator<CloseTopicCommand>
{
    public CloseTopicCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0).WithMessage("TopicId must be a positive number.");
    }
}
