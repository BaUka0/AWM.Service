using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;

/// <summary>
/// Validator for ApproveTopicCommand.
/// </summary>
public class ApproveTopicCommandValidator : AbstractValidator<ApproveTopicCommand>
{
    public ApproveTopicCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0).WithMessage("TopicId должен быть больше 0.");

        RuleFor(x => x.ApprovedBy)
            .GreaterThan(0).WithMessage("ApprovedBy должен быть больше 0.");
    }
}
