using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Topics.Commands.DeactivateTopic;

/// <summary>
/// Validator for DeactivateTopicCommand.
/// </summary>
public class DeactivateTopicCommandValidator : AbstractValidator<DeactivateTopicCommand>
{
    public DeactivateTopicCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0).WithMessage("TopicId должен быть больше 0.");
    }
}
