namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;

using FluentValidation;

/// <summary>
/// Validator for CloseTopicCommand.
/// </summary>
public sealed class CloseTopicCommandValidator : AbstractValidator<CloseTopicCommand>
{
    public CloseTopicCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("Topic ID must be greater than 0.");

        RuleFor(x => x.ClosedBy)
            .GreaterThan(0)
            .WithMessage("ClosedBy must be a valid user ID.");
    }
}